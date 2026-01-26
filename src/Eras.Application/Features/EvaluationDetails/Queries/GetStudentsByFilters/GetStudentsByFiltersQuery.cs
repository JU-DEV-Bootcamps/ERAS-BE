using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

using MediatR;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

public class GetStudentsByFiltersQuery : IRequest<List<StudentsByFiltersResponse>>
{
    public int? PollId { get; set; }
    public List<int>? ComponentIds { get; set; }
    public List<int>? CohortIds { get; set; }
    public List<int>? VariableIds { get; set; }

}