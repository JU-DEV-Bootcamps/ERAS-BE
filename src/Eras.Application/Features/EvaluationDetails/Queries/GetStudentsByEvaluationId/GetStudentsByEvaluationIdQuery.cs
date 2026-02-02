using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

using MediatR;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;

public class GetStudentsByEvaluationIdQuery : IRequest<List<StudentsByFiltersResponse>>
{
    public required int EvaluationId { get; set; }
    public required List<string> ComponentNames { get; set; }
    public required List<int> CohortIds { get; set; }
    public List<int>? VariableIds { get; set; }
    public List<int>? RiskLevels { get; set; }

}
