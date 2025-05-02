using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Calculations;

using MediatR;

namespace Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudentsByComponent;
public class GetCohortTopRiskStudentsByComponentQuery : IRequest<List<GetCohortTopRiskStudentsByComponentResponse>>
{
    public required string PollUuid { get; set; }
    public required string ComponentName { get; set; }
    public int CohortId { get; set; }
}
