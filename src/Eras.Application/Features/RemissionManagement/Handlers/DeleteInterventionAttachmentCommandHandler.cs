using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;

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
        string relativePath = Path.Combine(
            "interventions",
            request.InterventionId.ToString(),
            request.FileName);

        await _fileStorage.DeleteAsync(relativePath);

        await _repository.RemoveAttachmentAsync(request.InterventionId, relativePath);

        _logger.LogInformation(
            "Attachment '{FileName}' deleted from intervention '{Id}'.",
            request.FileName, request.InterventionId);
    }
}