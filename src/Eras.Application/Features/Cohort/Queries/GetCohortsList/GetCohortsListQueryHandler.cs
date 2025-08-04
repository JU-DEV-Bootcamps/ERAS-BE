using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortsList;

public class GetCohortsListQueryHandler(ICohortRepository Repository, ILogger<GetCohortsListQueryHandler> Logger) : IRequestHandler<GetCohortsListQuery, GetQueryResponse<List<Domain.Entities.Cohort>>>
{
    private readonly ICohortRepository _repository = Repository;
    private readonly ILogger<GetCohortsListQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<List<Domain.Entities.Cohort>>> Handle(GetCohortsListQuery Request, CancellationToken CancellationToken)
    {
        if (Request.PollUuid == string.Empty)
        {
            _logger.LogInformation("PollUuid is empty. Getting all cohorts");
            var res = await _repository.GetCohortsAsync();
            return new GetQueryResponse<List<Domain.Entities.Cohort>>(res, $"All {res.Count} Cohorts retrieved successfully", true);
        }
        List<Domain.Entities.Cohort> listOfCohorts = await _repository.GetCohortsByPollUuidAsync(Request.PollUuid, Request.LastVersion);
        return new GetQueryResponse<List<Domain.Entities.Cohort>>(listOfCohorts, $"{listOfCohorts} cohorts retrieved from poll {Request.PollUuid} successfully", true);
    }
}
