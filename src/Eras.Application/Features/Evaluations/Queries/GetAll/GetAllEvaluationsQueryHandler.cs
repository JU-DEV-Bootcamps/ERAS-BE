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
        private readonly ILogger<GetAllEvaluationsQueryHandler> _logger;

        public GetAllEvaluationsQueryHandler(IEvaluationRepository EvaluationRepository, ILogger<GetAllEvaluationsQueryHandler> Logger)
        {
            _evaluationRepository = EvaluationRepository;
            _logger = Logger;
        }
        public async Task<PagedResult<Evaluation>> Handle(GetAllEvaluationsQuery Request, CancellationToken CancellationToken)
        {
            var evaluations = await _evaluationRepository.GetPagedAsync(
                Request.Query.Page,
                Request.Query.PageSize
            );
            var totalCount = await _evaluationRepository.CountAsync();

            PagedResult<Evaluation> pagedResult = new PagedResult<Evaluation>(
                totalCount,
                evaluations.ToList()
            );

            return pagedResult;
        }
    }
}
