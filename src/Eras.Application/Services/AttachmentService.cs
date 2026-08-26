using System.Security.Cryptography;

using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.AttachmentManagement;
using Eras.Application.Models;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eras.Application.Services;

public sealed class AttachmentService(
    IAttachmentRepository Repository,
    IFileStorageService FileStorage,
    IOptions<FileStorageSettings> Settings,
    ILogger<AttachmentService> Logger) : IAttachmentService
{
    private readonly IAttachmentRepository _repository = Repository;
    private readonly IFileStorageService _fileStorage = FileStorage;
    private readonly FileStorageSettings _settings = Settings.Value;
    private readonly ILogger<AttachmentService> _logger = Logger;

    public async Task<AttachmentDto> UploadAttachmentAsync(
        string EntityType,
        int EntityId,
        Stream FileStream,
        string FileName,
        string CreatedBy,
        CancellationToken CancellationToken = default)
    {
        (AttachmentDto dto, _) = await UploadSingleAsync(EntityType, EntityId, FileStream, FileName, CreatedBy, CancellationToken);
        return dto;
    }

    public async Task<IReadOnlyCollection<AttachmentDto>> UploadAttachmentsAsync(
        string EntityType,
        int EntityId,
        IReadOnlyCollection<(Stream FileStream, string FileName)> Files,
        string CreatedBy,
        CancellationToken CancellationToken = default)
    {
        // Fail before any I/O if the whole batch is doomed anyway — same fail-fast behavior the
        // legacy per-entity handler had, avoided per-file since UploadSingleAsync also checks
        // these (a standalone single-file call still needs them), just redundant-but-cheap here.
        EnsureEntityTypeIsRegistered(EntityType);
        foreach ((_, var fileName) in Files)
            EnsureExtensionIsAllowed(fileName);

        var uploaded = new List<AttachmentDto>();
        var createdAttachmentIds = new List<int>();

        try
        {
            foreach ((Stream stream, var fileName) in Files)
            {
                (AttachmentDto dto, bool wasCreated) =
                    await UploadSingleAsync(EntityType, EntityId, stream, fileName, CreatedBy, CancellationToken);

                uploaded.Add(dto);
                if (wasCreated)
                    createdAttachmentIds.Add(dto.Id);
            }

            return uploaded;
        }
        catch
        {
            // A later file in the batch failed — roll back every attachment *this call* newly
            // created so the batch behaves atomically from the caller's point of view. Attachments
            // returned via dedup-match are deliberately left alone: this call didn't create them,
            // so it has no business deleting them.
            _logger.LogWarning(
                "Batch upload for {EntityType}/{EntityId} failed partway through; rolling back {Count} attachment(s) created by this request.",
                EntityType, EntityId, createdAttachmentIds.Count);

            foreach (int attachmentId in createdAttachmentIds)
            {
                try
                {
                    await DeleteAttachmentAsync(attachmentId, CancellationToken);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogError(cleanupEx,
                        "Failed to roll back attachment {AttachmentId} after a batch upload failure.", attachmentId);
                }
            }

            throw;
        }
    }

    /// <summary>Core single-file upload, shared by <see cref="UploadAttachmentAsync"/> and <see cref="UploadAttachmentsAsync"/>.</summary>
    /// <returns>The resulting DTO, and whether this call newly created it (false for a dedup match).</returns>
    private async Task<(AttachmentDto Dto, bool WasCreated)> UploadSingleAsync(
        string EntityType,
        int EntityId,
        Stream FileStream,
        string FileName,
        string CreatedBy,
        CancellationToken CancellationToken)
    {
        EnsureEntityTypeIsRegistered(EntityType);
        EnsureExtensionIsAllowed(FileName);
        EnsureSizeIsAllowed(FileStream);
        await EnsureContentMatchesExtensionAsync(FileStream, FileName, CancellationToken);

        (var contentHash, var sizeBytes) = await ComputeHashAndSizeAsync(FileStream, CancellationToken);
        FileStream.Position = 0;

        Attachment? existing = await _repository.GetByContentHashAsync(EntityType, EntityId, contentHash);
        if (existing is not null)
        {
            _logger.LogInformation(
                "Duplicate upload skipped for {EntityType}/{EntityId}: content hash already stored as attachment {AttachmentId}.",
                EntityType, EntityId, existing.Id);
            return (await ToDtoAsync(existing), false);
        }

        var maxAttachments = _settings.GetMaxAttachments(EntityType);
        var currentCount = await _repository.CountByEntityAsync(EntityType, EntityId);
        if (currentCount >= maxAttachments)
            throw new BussinessException(
                $"Entity '{EntityType}/{EntityId}' already has the maximum of {maxAttachments} attachments.", 409);

        var folder = AttachmentKeyScheme.BuildFolder(EntityType, EntityId);
        var storageKey = await _fileStorage.SaveAsync(FileStream, FileName, folder);

        try
        {
            var attachment = new Attachment
            {
                EntityType = EntityType,
                EntityId = EntityId,
                OriginalFileName = FileName,
                StorageKey = storageKey,
                MimeType = ContentTypeResolver.Resolve(FileName),
                SizeBytes = sizeBytes,
                ContentHash = contentHash,
                CreatedBy = CreatedBy
            };

            Attachment persisted = await _repository.AddAsync(attachment);
            return (await ToDtoAsync(persisted), true);
        }
        catch
        {
            // Metadata write failed after the physical file was already saved — compensate so no
            // orphaned file is left behind (the AC's "upload is transactional" requirement; disk
            // and Postgres can't share a real ACID transaction, so this is a best-effort rollback).
            _logger.LogWarning(
                "Attachment metadata write failed for {EntityType}/{EntityId}; deleting orphaned file {StorageKey}.",
                EntityType, EntityId, storageKey);
            await _fileStorage.DeleteAsync(storageKey);
            throw;
        }
    }

    public async Task<IReadOnlyCollection<AttachmentDto>> ListAttachmentsAsync(
        string EntityType,
        int EntityId,
        CancellationToken CancellationToken = default)
    {
        EnsureEntityTypeIsRegistered(EntityType);

        IEnumerable<Attachment> attachments = await _repository.GetByEntityAsync(EntityType, EntityId);

        var dtos = new List<AttachmentDto>();
        foreach (Attachment attachment in attachments)
            dtos.Add(await ToDtoAsync(attachment));

        return dtos;
    }

    public async Task<(Stream Stream, string? MimeType, string? OriginalFileName)> DownloadAttachmentAsync(
        int AttachmentId,
        CancellationToken CancellationToken = default)
    {
        Attachment attachment = await GetOrThrowAsync(AttachmentId);
        Stream stream = await _fileStorage.ReadAsync(attachment.StorageKey);
        return (stream, attachment.MimeType, attachment.OriginalFileName);
    }

    public async Task<string?> GetAttachmentUrlAsync(int AttachmentId, CancellationToken CancellationToken = default)
    {
        Attachment attachment = await GetOrThrowAsync(AttachmentId);
        return await _fileStorage.GetUrlAsync(attachment.StorageKey);
    }

    public async Task DeleteAttachmentAsync(int AttachmentId, CancellationToken CancellationToken = default)
    {
        Attachment attachment = await GetOrThrowAsync(AttachmentId);

        // Delete the metadata row first: if this fails, nothing has happened yet and the caller
        // can safely retry. Deleting the physical file first would risk a worse failure mode — a
        // metadata row pointing at a file that no longer exists, breaking future downloads.
        await _repository.DeleteAsync(attachment);

        try
        {
            await _fileStorage.DeleteAsync(attachment.StorageKey);
        }
        catch (Exception ex)
        {
            // The user-visible part (the metadata row) is already gone; a leaked physical file is
            // an ops/cleanup concern, not something to fail the delete operation over.
            _logger.LogWarning(ex,
                "Attachment {AttachmentId} metadata deleted but physical file {StorageKey} could not be removed.",
                AttachmentId, attachment.StorageKey);
        }
    }

    private async Task<Attachment> GetOrThrowAsync(int AttachmentId)
    {
        Attachment? attachment = await _repository.GetByIdAsync(AttachmentId);
        return attachment ?? throw new NotFoundException($"Attachment '{AttachmentId}' not found.");
    }

    private async Task<AttachmentDto> ToDtoAsync(Attachment Attachment)
    {
        string? downloadUrl = await _fileStorage.GetUrlAsync(Attachment.StorageKey);
        return new AttachmentDto
        {
            Id = Attachment.Id,
            EntityType = Attachment.EntityType,
            EntityId = Attachment.EntityId,
            OriginalFileName = Attachment.OriginalFileName,
            MimeType = Attachment.MimeType,
            SizeBytes = Attachment.SizeBytes,
            ContentHash = Attachment.ContentHash,
            CreatedAt = Attachment.CreatedAt,
            CreatedBy = Attachment.CreatedBy,
            DownloadUrl = downloadUrl
        };
    }

    private static void EnsureEntityTypeIsRegistered(string EntityType)
    {
        if (!AttachmentEntityTypeRegistry.IsRegistered(EntityType))
            throw new BussinessException($"Entity type '{EntityType}' is not registered for attachments.", 400);
    }

    private void EnsureExtensionIsAllowed(string FileName)
    {
        string extension = Path.GetExtension(FileName).ToLowerInvariant();
        if (!_settings.AllowedExtensions.Contains(extension))
            throw new BussinessException($"Extension '{extension}' is not allowed.", 400);
    }

    private void EnsureSizeIsAllowed(Stream FileStream)
    {
        // A metadata-only read (no content I/O), so this fails fast before wasting any work
        // hashing/peeking an oversized upload.
        if (FileStream.Length > _settings.MaxFileSizeBytes)
            throw new BussinessException(
                $"File size {FileStream.Length} bytes exceeds the maximum allowed size of {_settings.MaxFileSizeBytes} bytes.", 400);
    }

    /// <summary>
    /// Rejects content whose actual bytes don't match what <paramref name="FileName"/>'s extension
    /// claims — extension alone is never the sole validation signal (User Story 1.5). Resets
    /// <paramref name="FileStream"/> back to its start before returning.
    /// </summary>
    private static async Task EnsureContentMatchesExtensionAsync(Stream FileStream, string FileName, CancellationToken CancellationToken)
    {
        string extension = Path.GetExtension(FileName).ToLowerInvariant();
        byte[] header = new byte[FileSignatureValidator.HeaderBytesToRead];
        int bytesRead = await FileStream.ReadAsync(header.AsMemory(0, header.Length), CancellationToken);
        FileStream.Position = 0;

        if (!FileSignatureValidator.IsContentValidForExtension(header.AsSpan(0, bytesRead), extension))
            throw new BussinessException(
                $"File content does not match the expected format for extension '{extension}'.", 400);
    }

    private static async Task<(string ContentHash, long SizeBytes)> ComputeHashAndSizeAsync(
        Stream Stream, CancellationToken CancellationToken)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = await sha256.ComputeHashAsync(Stream, CancellationToken);
        return (Convert.ToHexString(hashBytes), Stream.Position);
    }

}
