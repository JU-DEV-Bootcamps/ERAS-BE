using Eras.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries
{
    class GetCohortsSummaryQueryHandler(
        IPollCohortRepository repository, 
        ILogger<GetCohortsSummaryQuery> logger) 
        : IRequestHandler<GetCohortsSummaryQuery, List<Eras.Domain.Entities.Cohort>>
    {
        private readonly IPollCohortRepository _repository = repository;
        private readonly ILogger<GetCohortsSummaryQuery> _logger = logger;

        public Task<List<Eras.Domain.Entities.Cohort>> Handle(GetCohortsSummaryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
