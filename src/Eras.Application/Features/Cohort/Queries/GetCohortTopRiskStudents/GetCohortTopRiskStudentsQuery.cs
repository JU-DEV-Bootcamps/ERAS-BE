using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQuery : IRequest<GetQueryResponse<PagedResult<GetCohortTopRiskStudentsByComponentResponse>>>
{
    public required string PollUuid { get; set; }
    public int CohortId { get; set; }
    public bool LastVersion { get; set; }
    public required Pagination PageValues { get; set; }
}
