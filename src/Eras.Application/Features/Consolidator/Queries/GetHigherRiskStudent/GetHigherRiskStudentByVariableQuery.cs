using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

/// <summary>
///  GetHigherRiskStudentQuery Returns a list of students with higher risk based on a sum of the risk in the poll answers.
// If {Take} is not provided, it will return 5 top risk students.
// If {CohortName} is provided, it will return the students in the cohort.
// {PollIDCosmicLatte} is required to get the poll answers.
/// </summary>
public class GetHigherRiskStudentByVariableQuery: IRequest<ListResponse<(Student, List<Answer>?, double)>>
{
    public required string CohortName { get; set; }
    public required int VariableId { get; set; }
    public required string PollInstanceUuid { get; set;}
    public int? Take { get; set; }
}
