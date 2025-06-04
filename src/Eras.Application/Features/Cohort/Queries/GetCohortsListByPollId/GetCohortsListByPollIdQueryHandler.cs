using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortsList;

public class GetCohortsListByPollIdQueryHandler(ICohortRepository Repository, ILogger<GetCohortsListQueryHandler> Logger) : IRequestHandler<GetCohortsListByPollIdQuery, GetQueryResponse<List<Cohort>>>
{
    private readonly ICohortRepository _repository = Repository;
    private readonly ILogger<GetCohortsListQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<List<Cohort>>> Handle(GetCohortsListByPollIdQuery Request, CancellationToken CancellationToken)
    {
        if (Request.PollId == null)
        {
            _logger.LogInformation("PollId is empty. Getting all cohorts");
            var res = await _repository.GetCohortsAsync();
            return new GetQueryResponse<List<Cohort>>(res, $"All {res.Count} Cohorts retrieved successfully", true);
        }
        List<Cohort> listOfCohorts = await _repository.GetCohortsByPollIdAsync(Request.PollId.Value);
        return new GetQueryResponse<List<Cohort>>(listOfCohorts, $"{listOfCohorts} cohorts retrieved from poll {Request.PollId} successfully", true);
    }
}
