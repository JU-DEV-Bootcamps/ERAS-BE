
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Models;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Eras.Application.Services;

public sealed class AttachmentRelocationService : IAttachmentRelocationService
{
    private readonly IAttachmentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly IOptions<FileStorageSettings> _settings;
    private readonly ILogger<AttachmentRelocationService> _logger;

    public AttachmentRelocationService(
        IAttachmentRepository Repository,
        IFileStorageService FileStorage,
        IOptions<FileStorageSettings> Settings,
        ILogger<AttachmentRelocationService> Logger)
    {
        _repository = Repository;
        _fileStorage = FileStorage;
        _settings = Settings;
        _logger = Logger;
    }
    public async Task RunAsync(CancellationToken CancellationToken = default)
    {
        IReadOnlyCollection<Attachment> pending = await _repository.GetPendingRelocationAsync(_settings.Value.StorageRelocationBatchSize);
        if (pending.Count == 0)
            return;

        _logger.LogInformation("Storage relocation sweep: {Count} attachment(s) pending.", pending.Count);

        foreach (Attachment attachment in pending)
        {
            CancellationToken.ThrowIfCancellationRequested();
            await RelocateOneAsync(attachment);
        }
    }

    private async Task RelocateOneAsync(Attachment attachment)
    {
        string oldKey = attachment.StorageKey;
        string newKey = BuildDestinationKey(attachment);

        try
        {
            if (await _fileStorage.ExistsAsync(oldKey))
            {
                await _fileStorage.MoveAsync(oldKey, newKey);
                _logger.LogInformation(
                    "Attachment {AttachmentId} relocated: {OldKey} -> {NewKey}", attachment.Id, oldKey, newKey);
            }
            else
            {
                _logger.LogWarning(
                    "Attachment {AttachmentId}: source key {OldKey} not found — treating relocation as already done.",
                    attachment.Id, oldKey);
            }

            await _repository.MarkRelocatedAsync(attachment.Id, newKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Attachment {AttachmentId}: relocation from {OldKey} to {NewKey} failed; left pending.",
                attachment.Id, oldKey, newKey);
        }
    }

    private static string BuildDestinationKey(Attachment Attachment)
    {
        string folder = AttachmentKeyScheme.BuildFolder(Attachment.EntityType, Attachment.EntityId);
        string fileNameSegment = Path.GetFileName(Attachment.StorageKey);
        return Path.Combine(folder, fileNameSegment).Replace('\\', '/');
    }
}