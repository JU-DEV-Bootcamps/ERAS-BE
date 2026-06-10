using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

public class GetStudentsByFiltersQuery : IRequest<PagedResult<StudentsByFiltersResponse>>
{
    public required string PollUuid { get; set; }
    public required List<string> ComponentNames { get; set; }
    public required List<int> CohortIds { get; set; }
    public List<int>? VariableIds { get; set; }
    public List<decimal>? RiskLevels { get; set; }
    public required Pagination PageValues { get; set; } 

}