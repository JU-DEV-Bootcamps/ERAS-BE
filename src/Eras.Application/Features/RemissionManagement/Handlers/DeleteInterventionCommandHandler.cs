using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class DeleteInterventionCommandHandler : IRequestHandler<DeleteInterventionCommand>
{
    private readonly IAssessmentRepository _repository;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<DeleteInterventionCommandHandler> _logger;

    public DeleteInterventionCommandHandler(
        IAssessmentRepository repository,
        IFileStorageService fileStorage,
        ILogger<DeleteInterventionCommandHandler> logger)
    {
        _repository = repository;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task Handle(DeleteInterventionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var intervention = await _repository.GetInterventionByIdAsync(request.InterventionId);

            if (intervention is not null)
            {
                foreach (string attachment in intervention.Attachments)
                {
                    try
                    {
                        await _fileStorage.DeleteAsync(attachment);
                    }
                    catch (FileNotFoundException)
                    {
                        _logger.LogWarning(
                            "Attachment '{Path}' not found in storage during intervention deletion.",
                            attachment);
                    }
                }
            }

            await _repository.DeleteInterventionAsync(request.AssessmentId, request.InterventionId);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Intervention '{InterventionId}' not found for assessment '{AssessmentId}'.",
                request.InterventionId, request.AssessmentId);
            throw;
        }
    }
}