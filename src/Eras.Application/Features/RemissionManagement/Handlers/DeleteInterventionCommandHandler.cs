using Eras.Application.Contracts.Persistence.AssessmentManagement;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class DeleteInterventionCommandHandler : IRequestHandler<DeleteInterventionCommand>
{
    private readonly IAssessmentRepository _repository;
    private readonly ILogger<DeleteInterventionCommandHandler> _logger;

    public DeleteInterventionCommandHandler(
        IAssessmentRepository repository,
        ILogger<DeleteInterventionCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task Handle(DeleteInterventionCommand request, CancellationToken cancellationToken)
    {
        try
        {
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