using Eras.Application.Contracts.Persistence.AssessmentManagement;
using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class DeleteInterventionCommandHandler
    : IRequestHandler<DeleteInterventionCommand>
{
    private readonly IAssessmentRepository _repository;

    public DeleteInterventionCommandHandler(IAssessmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteInterventionCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.DeleteInterventionAsync(request.AssessmentId, request.InterventionId);
    }
}