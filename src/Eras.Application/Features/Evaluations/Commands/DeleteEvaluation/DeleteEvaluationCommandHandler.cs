using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

namespace Eras.Application.Features.Evaluations.Commands.DeleteEvaluation
{
    public class DeleteEvaluationCommandHandler : IRequestHandler<DeleteEvaluationCommand, BaseResponse>
    {
        private readonly IEvaluationRepository _evaluationRepository;

        public DeleteEvaluationCommandHandler(IEvaluationRepository EvaluationRepository)
        {
            _evaluationRepository = EvaluationRepository;
        }

        public async Task<BaseResponse> Handle(DeleteEvaluationCommand Request, CancellationToken CancellationToken)
        {
            Evaluation? evaluation =
                await _evaluationRepository.GetByIdForUpdateAsync(Request.id)
                ?? throw new NotFoundException($"Evaluation with ID {Request.id} not found");

            await _evaluationRepository.DeleteAsync(evaluation);
            return new BaseResponse("Evaluation deleted", true);
        }
    }
}
