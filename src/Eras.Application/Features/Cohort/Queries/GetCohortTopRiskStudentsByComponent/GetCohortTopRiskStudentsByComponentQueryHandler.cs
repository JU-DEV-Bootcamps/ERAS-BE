using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
public class GetCohortTopRiskStudentsByComponentQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> Logger) : IRequestHandler<GetCohortTopRiskStudentsByComponentQuery, GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>>
{
    private readonly ICohortRepository _cohortRepository = CohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>> Handle(GetCohortTopRiskStudentsByComponentQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var listStudents = await _cohortRepository.GetCohortTopRiskStudentsByComponentAsync(
                Request.PollUuid,
                Request.ComponentName,
                Request.CohortId,
                Request.LastVersion,
                Request.PageValues.Page,
                Request.PageValues.PageSize
            );

            var totalCount = await _cohortRepository.CountStudentsAsync(Request.PollUuid, Request.CohortId, Request.LastVersion, Request.ComponentName);
            var pagedResult = new PagedResult<GetCohortTopRiskStudentsByComponentResponse>(totalCount, listStudents.ToList());
            return new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting cohort top risk: " + ex.Message);
            return new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(
                new PagedResult<GetCohortTopRiskStudentsByComponentResponse>(0, []));
        }
    }
}
