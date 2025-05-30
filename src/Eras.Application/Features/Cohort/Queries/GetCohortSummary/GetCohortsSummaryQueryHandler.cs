using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries
{
    class GetCohortsSummaryQueryHandler(
        IStudentCohortRepository ScRepository,
        ILogger<GetCohortsSummaryQuery> Logger)
        : IRequestHandler<GetCohortsSummaryQuery, List<(Student Student, List<PollInstance> PollInstances)>>
    {
        private readonly IStudentCohortRepository _screpository = ScRepository;
        private readonly ILogger<GetCohortsSummaryQuery> _logger = Logger;

        public async Task<List<(Student Student, List<PollInstance> PollInstances)>> Handle(
            GetCohortsSummaryQuery Request,
            CancellationToken CancellationToken)
        {
            var cohortSummary = await _screpository.GetCohortsSummaryAsync();
            return cohortSummary;
        }
    }
}
