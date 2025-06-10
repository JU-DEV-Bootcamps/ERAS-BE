using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
public class GetCohortTopRiskStudentsByComponentQueryHandler(ICohortRepository CohortRepository, ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> Logger) : IRequestHandler<GetCohortTopRiskStudentsByComponentQuery, GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>>
{
    private readonly ICohortRepository _cohortRepository = CohortRepository;
    private readonly ILogger<GetCohortTopRiskStudentsByComponentQueryHandler> _logger = Logger;

    public async Task<GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>> Handle(GetCohortTopRiskStudentsByComponentQuery Request, CancellationToken CancellationToken)
    {
        List<GetCohortTopRiskStudentsByComponentResponse> listStudents = await _cohortRepository.GetCohortTopRiskStudentsByComponentAsync(Request.PollUuid, Request.ComponentName, Request.CohortId, Request.LastVersion);
        return new GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>(listStudents);
    }
}
