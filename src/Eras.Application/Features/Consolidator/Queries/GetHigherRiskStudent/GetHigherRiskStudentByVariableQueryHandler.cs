using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentByVariableQueryHandler(
    ILogger<GetHigherRiskStudentByVariableQueryHandler> logger,
    IPollVariableRepository pollVariableRepository
  ) : IRequestHandler<GetHigherRiskStudentByVariableQuery, GetQueryResponse<List<(Student student, List<Answer> answers, List<Variable> variables, double riskIndex)>>>
{
    private readonly ILogger<GetHigherRiskStudentByVariableQueryHandler> _logger = logger;
    private readonly IPollVariableRepository _pollVariableRepository = pollVariableRepository;
    public int DefaultTakeNumber = 5;

    public async Task<GetQueryResponse<List<(Student student, List<Answer> answers, List<Variable> variables, double riskIndex)>>> Handle(GetHigherRiskStudentByVariableQuery request, CancellationToken cancellationToken)
    {
        try{
            var results = await _pollVariableRepository.GetByPollUuidAsync(request.PollInstanceUuid, request.VariableId);
            var byStudent = results.GroupBy(r => r.Student)
                   .Select(g => (
                        student: g.Key,
                        answers: results.Where(r => r.Student.Id == g.Key.Id).Select(r => r.Answer).ToList(),
                        variables: results.Where(r => r.Student.Id == g.Key.Id).Select(r => r.Variable).ToList(),
                        riskIndex: g.Average(s => s.Answer.RiskLevel)
                   )).OrderByDescending(o => o.riskIndex)
                   .Take(request.Take ?? DefaultTakeNumber)
                   .ToList();
            return new GetQueryResponse<List<(Student student, List<Answer> answers, List<Variable> variables, double riskIndex)>>(byStudent, "Success", true);
        }
        catch(Exception e){
            _logger.LogError(e, "An error occurred while calculating higher risk students: " + request);
            return new GetQueryResponse<List<(Student student, List<Answer> answers, List<Variable> variables, double riskIndex)>>([], $"Failed to retrieve top risk students by variable. Error {e.Message}", false);
        }
    }
}
