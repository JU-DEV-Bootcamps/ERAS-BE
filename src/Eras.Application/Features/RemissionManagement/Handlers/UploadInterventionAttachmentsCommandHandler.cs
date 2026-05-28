using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Models;

using MediatR;

using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UploadInterventionAttachmentsCommandHandler
    : IRequestHandler<UploadInterventionAttachmentsCommand, IReadOnlyCollection<string>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly FileStorageSettings _settings;
    private readonly ILogger<UploadInterventionAttachmentsCommandHandler> _logger; 

    public UploadInterventionAttachmentsCommandHandler(
        IAssessmentRepository repository,
        IFileStorageService fileStorage,
        IOptions<FileStorageSettings> settings,
        ILogger<UploadInterventionAttachmentsCommandHandler> logger)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<string>> Handle(
    UploadInterventionAttachmentsCommand request,
    CancellationToken cancellationToken)
    {
        foreach ((_, string fileName) in request.Files)
        {
            string ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedExtensions.Contains(ext))
                throw new InvalidOperationException($"Extension '{ext}' is not allowed.");
        }

        string folder = $"interventions/{request.InterventionId}";

        IReadOnlyCollection<string> existingHashes =
            await _repository.GetAttachmentHashesAsync(request.InterventionId, cancellationToken);

        List<string> savedPaths = [];
        List<string> savedHashes = [];

        foreach ((Stream stream, string fileName) in request.Files)
        {
            string hash = await ComputeSha256Async(stream);
            stream.Position = 0;

            if (existingHashes.Contains(hash))
            {
                _logger.LogInformation("Duplicate file skipped: {FileName}", fileName);
                continue;
            }

            string path = await _fileStorage.SaveAsync(stream, fileName, folder);
            savedPaths.Add(path);
            savedHashes.Add(hash);
        }

        if (savedPaths.Count > 0)
            await _repository.AddAttachmentsAsync(request.InterventionId, savedPaths, savedHashes);

        return savedPaths.AsReadOnly();
    }

    private static async Task<string> ComputeSha256Async(Stream stream)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        byte[] hashBytes = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hashBytes);
    }
}