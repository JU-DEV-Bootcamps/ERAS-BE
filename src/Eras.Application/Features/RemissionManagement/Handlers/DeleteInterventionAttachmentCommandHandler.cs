using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class DeleteInterventionAttachmentCommandHandler : IRequestHandler<DeleteInterventionAttachmentCommand>
{
    private readonly IAssessmentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DeleteInterventionAttachmentCommandHandler> _logger;

    public DeleteInterventionAttachmentCommandHandler(
        IAssessmentRepository repository,
        IFileStorageService fileStorage,
        ILogger<DeleteInterventionAttachmentCommandHandler> logger)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task Handle(DeleteInterventionAttachmentCommand request, CancellationToken cancellationToken)
    {
        Intervention? intervention = await _repository.GetInterventionByIdAsync(request.InterventionId);

        if (intervention is null)
            throw new KeyNotFoundException($"Intervention '{request.InterventionId}' not found.");

        string fileNameToRemove = Path.GetFileName(request.FileName);

        bool attachmentExists = intervention.Attachments
            .Any(a => Path.GetFileName(a).Equals(fileNameToRemove, StringComparison.OrdinalIgnoreCase));

        if (!attachmentExists)
            throw new KeyNotFoundException($"Attachment '{request.FileName}' not found in intervention '{request.InterventionId}'.");

        string relativePath = Path.Combine(
            "interventions",
            request.InterventionId.ToString(),
            request.FileName).Replace('\\', '/');

        await _fileStorage.DeleteAsync(relativePath);

        await _repository.RemoveAttachmentAsync(request.InterventionId, relativePath);

        _logger.LogInformation(
            "Attachment '{FileName}' deleted from intervention '{Id}'.",
            request.FileName, request.InterventionId);
    }
}