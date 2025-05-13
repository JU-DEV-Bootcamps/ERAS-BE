using Eras.Application.Contracts.Persistence;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortsList;

public class GetCohortsListQueryHandler(ICohortRepository Repository, ILogger<GetCohortsListQueryHandler> Logger) : IRequestHandler<GetCohortsListQuery, List<Domain.Entities.Cohort>>
{
    private readonly ICohortRepository _repository = Repository;
    private readonly ILogger<GetCohortsListQueryHandler> _logger = Logger;

    public async Task<List<Domain.Entities.Cohort>> Handle(GetCohortsListQuery Request, CancellationToken CancellationToken)
    {
        if (Request.PollUuid == string.Empty)
        {
            _logger.LogError("PollUuid is empty. Getting all cohorts");
            return await _repository.GetCohortsAsync();
        }
        List<Domain.Entities.Cohort> listOfCohorts = await _repository.GetCohortsByPollUuidAsync(Request.PollUuid);
        return listOfCohorts;
    }
}
