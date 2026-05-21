using Eras.Application.Contracts.Persistence.AssessmentManagement;
using MediatR;

namespace Eras.Application.Features.RemissionManagement.Handlers;

public sealed class DeleteAssessmentCommandHandler
    : IRequestHandler<DeleteAssessmentCommand>
{
    private readonly IAssessmentRepository _repository;

    public DeleteAssessmentCommandHandler(IAssessmentRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteAssessmentCommand request,
        CancellationToken cancellationToken)
    {
        await _repository.DeleteAssessmentAsync(request.AssessmentId);
    }
}