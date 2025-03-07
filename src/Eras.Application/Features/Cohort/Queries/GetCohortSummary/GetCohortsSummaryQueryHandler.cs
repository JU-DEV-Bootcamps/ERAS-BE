using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries
{
    class GetCohortsSummaryQueryHandler(
        IStudentCohortRepository scRepository,
        ILogger<GetCohortsSummaryQuery> logger)
        : IRequestHandler<GetCohortsSummaryQuery, List<(Student Student, List<PollInstance> PollInstances)>>
    {
        private readonly IStudentCohortRepository _screpository = scRepository;
        private readonly ILogger<GetCohortsSummaryQuery> _logger = logger;

        public async Task<List<(Student Student, List<PollInstance> PollInstances)>> Handle(GetCohortsSummaryQuery request, CancellationToken cancellationToken)
        {
            var cohortSummary = await _screpository.GetCohortsSummaryAsync();
            return cohortSummary;
        }
    }
}
