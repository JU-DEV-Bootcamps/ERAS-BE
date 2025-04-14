using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentByVariableQueryHandler(
    ILogger<GetHigherRiskStudentByVariableQueryHandler> logger,
    IPollVariableRepository pollVariableRepository
  ) : IRequestHandler<GetHigherRiskStudentByVariableQuery, GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>>
{
    private readonly ILogger<GetHigherRiskStudentByVariableQueryHandler> _logger = logger;
    private readonly IPollVariableRepository _pollVariableRepository = pollVariableRepository;
    public int DefaultTakeNumber = 5;

    public async Task<GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>> Handle(GetHigherRiskStudentByVariableQuery request, CancellationToken cancellationToken)
    {
        int TakeNStudents = request.Take.HasValue && request.Take.Value > 0 ? request.Take.Value : DefaultTakeNumber;
        try{
            var results = await _pollVariableRepository.GetByPollUuidAsync(request.PollInstanceUuid, request.VariableId);
            var orderedStudents = results.OrderByDescending(s => s.Answer.RiskLevel).Take(TakeNStudents).ToList();
            return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(orderedStudents, "Success", true);
        }
        catch(Exception e){
            _logger.LogError(e, "An error occurred while calculating higher risk students: " + request);
            return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(
                new List<(Answer answer, Variable variable, Student student)>(),
                $"Failed to retrieve top risk students by variable. Error {e.Message}",
                false
            );
        }
    }
}
