using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries.GetAll
{
    public class GetAllEvaluationsQueryHandler : IRequestHandler<GetAllEvaluationsQuery, PagedResult<Evaluation>>
    {
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IImportJobRepository _importJobRepository;
        private readonly ILogger<GetAllEvaluationsQueryHandler> _logger;

        public GetAllEvaluationsQueryHandler(
            IEvaluationRepository EvaluationRepository,
            IImportJobRepository ImportJobRepository,
            ILogger<GetAllEvaluationsQueryHandler> Logger)
        {
            _evaluationRepository = EvaluationRepository;
            _importJobRepository = ImportJobRepository;
            _logger = Logger;
        }

        public async Task<PagedResult<Evaluation>> Handle(GetAllEvaluationsQuery Request, CancellationToken CancellationToken)
        {
            var evaluations = Request.Query != null
                ? await _evaluationRepository.GetPagedAsync(
                    Request.Query.Page,
                    Request.Query.PageSize
                )
                : await _evaluationRepository.GetAllAsync();

            var evaluationList = evaluations.ToList();
            var totalCount = await _evaluationRepository.CountAsync();

            var evaluationIds = evaluationList.Select(e => e.Id).ToList();
            var latestImportJobIds = await _importJobRepository
                .GetLatestImportJobIdsByEvaluationIdsAsync(evaluationIds);

            foreach (var evaluation in evaluationList)
            {
                evaluation.LatestImportJobId = latestImportJobIds
                    .TryGetValue(evaluation.Id, out var jobId)
                        ? jobId
                        : null;
            }

            PagedResult<Evaluation> pagedResult = new PagedResult<Evaluation>(
                totalCount,
                evaluationList
            );

            return pagedResult;
        }
    }
}