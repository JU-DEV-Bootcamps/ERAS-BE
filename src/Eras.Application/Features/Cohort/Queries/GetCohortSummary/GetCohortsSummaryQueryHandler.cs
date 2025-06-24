using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.CohortsController;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries
{
    class GetCohortsSummaryQueryHandler(
        IStudentCohortRepository ScRepository,
        ILogger<GetCohortsSummaryQuery> Logger)
        : IRequestHandler<GetCohortsSummaryQuery, CohortSummaryResponse>
    {
        private readonly IStudentCohortRepository _screpository = ScRepository;
        private readonly ILogger<GetCohortsSummaryQuery> _logger = Logger;

        public async Task<CohortSummaryResponse> Handle(
            GetCohortsSummaryQuery Request,
            CancellationToken CancellationToken)
        {
            var cohortSummary = await _screpository.GetCohortsSummaryAsync(Request.Pagination);
            return cohortSummary;
        }
    }
}
