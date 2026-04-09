using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.CohortsController;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries
{
    class GetCohortsSummaryQueryHandler(
        IStudentCohortRepository ScRepository,
        IEvaluationRepository EvaluationRepository, 
        ILogger<GetCohortsSummaryQuery> Logger)
        : IRequestHandler<GetCohortsSummaryQuery, CohortSummaryResponse>
    {
        private readonly IStudentCohortRepository _screpository = ScRepository;
        private readonly IEvaluationRepository _evaluationRepository = EvaluationRepository;
        private readonly ILogger<GetCohortsSummaryQuery> _logger = Logger;

        public async Task<CohortSummaryResponse> Handle(
            GetCohortsSummaryQuery Request,
            CancellationToken CancellationToken)
        {
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (Request.EvaluationId.HasValue)
            {
                var evaluation = await _evaluationRepository.GetByIdAsync(Request.EvaluationId.Value);
                startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc)
                                .Date.AddDays(1).AddTicks(-1);
            }

            return await _screpository.GetCohortsSummaryAsync(Request.Pagination, startDate, endDate);
        }
    }
}
