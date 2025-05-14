using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortsList;

public class GetCohortsListQueryHandler(ICohortRepository Repository, ILogger<GetCohortsListQueryHandler> Logger) : IRequestHandler<GetCohortsListQuery, GetQueryResponse<List<Domain.Entities.Cohort>>>
{
    private readonly ICohortRepository _repository = Repository;
    private readonly ILogger<GetCohortsListQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<List<Domain.Entities.Cohort>>> Handle(GetCohortsListQuery Request, CancellationToken CancellationToken)
    {
        if (Request.PollUuid == string.Empty)
        {
            _logger.LogError("PollUuid is empty. Getting all cohorts");
            return new GetQueryResponse<List<Domain.Entities.Cohort>>(await _repository.GetCohortsAsync());
        }
        List<Domain.Entities.Cohort> listOfCohorts = await _repository.GetCohortsByPollUuidAsync(Request.PollUuid);
        return new GetQueryResponse<List<Domain.Entities.Cohort>>(listOfCohorts);
    }
}
