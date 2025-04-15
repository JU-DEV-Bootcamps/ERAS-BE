using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Polls;

public class GetPollTopQueryHandler(
 ILogger<GetPollTopQueryHandler> Logger,
 IPollVariableRepository PollVariableRepository
) : IRequestHandler<GetPollTopQuery, GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>>
{
    private readonly ILogger<GetPollTopQueryHandler> _logger = Logger;
    private readonly IPollVariableRepository _pollVariableRepository = PollVariableRepository;
    public int DefaultTakeNumber = 9999;

    public async Task<GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>> Handle(
        GetPollTopQuery Request,
        CancellationToken CancellationToken)
    {
        var TakeNStudents = Request.Take > 0 ? Request.Take : DefaultTakeNumber;
        try
        {
            List<(Answer Answer, Variable Variable, Student Student)> results = await _pollVariableRepository.GetByPollUuidAsync(Request.PollUuid.ToString(), Request.VariableIds);
            var orderedStudents = results.OrderByDescending(S => S.Answer.RiskLevel).Take(TakeNStudents).ToList();
            return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(orderedStudents, "Success", true);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while calculating higher risk students: " + Request);
            return new GetQueryResponse<List<(Answer answer, Variable variable, Student student)>>(
                [],
                $"Failed to retrieve top risk students by variable. Error {e.Message}",
                false
            );
        }
    }
}
