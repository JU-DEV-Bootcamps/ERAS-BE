using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudentsByComponent;
public class GetCohortTopRiskStudentsByComponentQuery : IRequest<GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>>
{
    public required string PollUuid { get; set; }
    public required string ComponentName { get; set; }
    public int CohortId { get; set; }
    public bool LastVersion { get; set; }
}
