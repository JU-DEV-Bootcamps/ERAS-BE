using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQueryHandler : IRequestHandler<GetCohortTopRiskStudentsQuery, GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>>
{
    private readonly ICohortRepository _cohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsQueryHandler> _logger;
    public GetCohortTopRiskStudentsQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsQueryHandler> Logger)
    {
        _cohortRepository = CohortRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>> Handle(GetCohortTopRiskStudentsQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var studentsList = await _cohortRepository.GetCohortTopRiskStudentsAsync(
                Request.PollUuid,
                Request.CohortId,
                Request.LastVersion,
                Request.PageValues.Page,
                Request.PageValues.PageSize
            );

            var totalCount = await _cohortRepository.CountStudentsAsync(Request.PollUuid, Request.CohortId, Request.LastVersion);
            var pagedResult = new PagedResult<GetCohortTopRiskStudentsByComponentResponse>(totalCount, studentsList.ToList());
            return new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(pagedResult);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while getting cohort top risk: " + ex.Message);
            return new GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>(
                new PagedResult<GetCohortTopRiskStudentsByComponentResponse>(0, []));
        }
    }
}
