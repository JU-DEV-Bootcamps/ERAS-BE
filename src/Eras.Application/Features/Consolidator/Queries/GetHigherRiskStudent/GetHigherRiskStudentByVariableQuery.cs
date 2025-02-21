using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

/// <summary>
///  GetHigherRiskStudentQuery Returns a list of students with higher risk based on a sum of the risk in the poll answers.
// If {Take} is not provided, it will return 5 top risk students.
// {PollIDCosmicLatte} is required to get the poll answers.
// {VariableId} is required to filter the answers.
/// </summary>
public class GetHigherRiskStudentByVariableQuery: IRequest<GetQueryResponse<List<(Student student, List<Answer> answers, List<Variable> variables, double riskIndex)>>>
{
    public required int VariableId { get; set; }
    public required string PollInstanceUuid { get; set;}
    public int? Take { get; set; }
}
