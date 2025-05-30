using Eras.Application.Models.Response.Calculations;
using Eras.Application.Models.Response.Common;

using MediatR;

namespace Eras.Application.Features.Cohorts.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQuery : IRequest<GetQueryResponse<List<GetCohortTopRiskStudentsByComponentResponse>>>
{
    public required string PollUuid { get; set; }
    public int CohortId { get; set; }
}
