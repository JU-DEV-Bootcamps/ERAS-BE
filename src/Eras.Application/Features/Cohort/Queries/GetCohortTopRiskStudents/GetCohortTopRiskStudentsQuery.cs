using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Calculations;
using MediatR;

namespace Eras.Application.Features.Cohort.Queries.GetCohortTopRiskStudents;
public class GetCohortTopRiskStudentsQuery : IRequest<List<GetCohortTopRiskStudentsByComponentResponse>>
{
    public required string PollUuid { get; set; }
    public int CohortId { get; set; }
}
