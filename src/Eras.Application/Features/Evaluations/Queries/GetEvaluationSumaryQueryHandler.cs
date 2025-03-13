using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(
        IPollCohortRepository repository,
        IEvaluationPollRepository evaluationPollRepository,
        ILogger<GetEvaluationSummaryQuery> logger
    ): IRequestHandler<GetEvaluationSummaryQuery, List<Poll>>
    {
        public async Task<List<Poll>> Handle(GetEvaluationSummaryQuery request, CancellationToken cancellationToken)
        {
            logger.LogDebug("Handling summarizing all evaluation processes");
            var polls = await repository.GetAllAsync();
            var evProcesses = await evaluationPollRepository.GetAllAsync();
            return [.. polls];
        }
    }
}
