using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Consolidator.Queries.Students;

/// <summary>
///  GetHigherRiskStudentQuery Returns a list of students with higher risk based on a sum of the risk in the poll answers.
// If {Take} is not provided, it will return 5 top risk students.
// If {CohortName} is provided, it will return the students in the cohort.
// {PollName} is required to get the poll answers.
/// </summary>
public class GetStudentTopQuery : IRequest<GetQueryResponse<List<(Student Student, List<Answer> Answers, double RiskIndex)>>>
{
    public required string CohortName { get; set; }
    public required string PollName { get; set; }
    public int? Take { get; set; } = 5;
}
