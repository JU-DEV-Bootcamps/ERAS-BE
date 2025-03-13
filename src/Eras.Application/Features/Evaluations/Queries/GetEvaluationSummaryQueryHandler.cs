using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(
        IEvaluationPollRepository evaluationPollRepository,
        ILogger<GetEvaluationProcessSumaryQueryHandler> logger
    ): IRequestHandler<GetEvaluationSummaryQuery, QueryManyResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository = evaluationPollRepository;
        private readonly ILogger<GetEvaluationProcessSumaryQueryHandler> _logger = logger;


        Task<QueryManyResponse<Evaluation>> IRequestHandler<GetEvaluationSummaryQuery, QueryManyResponse<Evaluation>>.Handle(GetEvaluationSummaryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling summarizing all evaluation processes");
            var evProcesses = _evaluationPollRepository.GetAllPollsPollInstances().ToList();
            return Task.FromResult(new QueryManyResponse<Evaluation>(evProcesses, "Summary", true));

        }
    }
}
