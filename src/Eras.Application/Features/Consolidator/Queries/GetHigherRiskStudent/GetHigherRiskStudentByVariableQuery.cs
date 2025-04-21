using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

/// <summary>
///  GetHigherRiskStudentQuery Returns a list of students with higher risk based on a sum of the risk in the poll answers.
// If {Take} is not provided, it will return 5 top risk students.
// {PollInstanceUuid} is required to get the poll answers.
// {VariableId} is required to filter the answers.
/// </summary>
public class GetHigherRiskStudentByVariableQuery: IRequest<GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>>
{
    public required int VariableId { get; set; }
    public required string PollInstanceUuid { get; set;}
    public int? Take { get; set; }
}
