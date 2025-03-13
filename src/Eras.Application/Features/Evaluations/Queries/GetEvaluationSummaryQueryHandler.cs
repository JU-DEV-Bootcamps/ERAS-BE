using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(
        IEvaluationPollRepository evaluationPollRepository,
        IEvaluationRepository evaluationRepository,
        ILogger<GetEvaluationProcessSumaryQueryHandler> logger
    ): IRequestHandler<GetEvaluationSummaryQuery, QueryManyResponse<Evaluation>>
    {
        private readonly IEvaluationPollRepository _evaluationPollRepository = evaluationPollRepository;
        private readonly IEvaluationRepository _evaluationRepository = evaluationRepository;
        private readonly ILogger<GetEvaluationProcessSumaryQueryHandler> _logger = logger;


        Task<QueryManyResponse<Evaluation>> IRequestHandler<GetEvaluationSummaryQuery, QueryManyResponse<Evaluation>>.Handle(GetEvaluationSummaryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling summarizing all evaluation processes");
            var evs = _evaluationRepository.GetAllAsync().Result.ToList();
            return Task.FromResult(new QueryManyResponse<Evaluation>(evs, "Summary", true));
        }
    }
}
