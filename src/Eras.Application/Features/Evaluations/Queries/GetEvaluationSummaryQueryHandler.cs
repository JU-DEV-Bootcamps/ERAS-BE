using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(
        IEvaluationPollRepository evaluationPollRepository,
        ILogger<GetEvaluationSummaryQuery> logger
    ): IRequestHandler<GetEvaluationSummaryQuery, List<Evaluation>>
    {
        public Task<List<Evaluation>> Handle(GetEvaluationSummaryQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Handling summarizing all evaluation processes");
            var evProcesses = evaluationPollRepository.GetAllPollsPollInstances().ToList();
            return Task.FromResult(evProcesses);
        }
    }
}
