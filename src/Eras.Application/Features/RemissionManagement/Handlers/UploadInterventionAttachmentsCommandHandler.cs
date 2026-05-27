using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Models;

using MediatR;

using Microsoft.Extensions.Options;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class UploadInterventionAttachmentsCommandHandler
    : IRequestHandler<UploadInterventionAttachmentsCommand, IReadOnlyCollection<string>>
{
    private readonly IAssessmentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly FileStorageSettings _settings;

    public UploadInterventionAttachmentsCommandHandler(
        IAssessmentRepository repository,
        IFileStorageService fileStorage,
        IOptions<FileStorageSettings> settings)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _settings = settings.Value;
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

        List<string> savedPaths = [];
        foreach ((Stream stream, string fileName) in request.Files)
        {
            string path = await _fileStorage.SaveAsync(stream, fileName, folder);
            savedPaths.Add(path);
        }

        await _repository.AddAttachmentsAsync(request.InterventionId, savedPaths);

        return savedPaths.AsReadOnly();
    }
}