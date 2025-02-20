using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentByVariableQueryHandler(
    ICohortRepository cohortRepository,
    IStudentRepository studentRepository,
    IAnswerRepository answerRepository,
    IPollVariableRepository pollVariableRepository,
    IVariableRepository variableRepository,
    IPollInstanceRepository pollInstanceRepository,
    ILogger<GetHigherRiskStudentByVariableQueryHandler> logger
  ) : IRequestHandler<GetHigherRiskStudentByVariableQuery, ListResponse<(Student, List<Answer, Variable>?, double)?>>
{
    private readonly ICohortRepository _cohortRepository = cohortRepository;
    private readonly IStudentRepository _studentRepository = studentRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;

    private readonly IVariableRepository _variableRepository = variableRepository;
    private readonly IPollVariableRepository _pollVariableRepository = pollVariableRepository;
    private readonly IPollInstanceRepository _pollInstanceRepository = pollInstanceRepository;
    private readonly ILogger<GetHigherRiskStudentByVariableQueryHandler> _logger = logger;
    public int DefaultTakeNumber = 5;

    public async Task<ListResponse<(Student, List<(Answer, Variable)>, double)>> Handle(GetHigherRiskStudentByVariableQuery request, CancellationToken cancellationToken)
    {
        try {
            int TakeNStudents = request.Take ?? DefaultTakeNumber;
            var answersVariables = await _pollVariableRepository.GetByPollUuidAsync(request.PollInstanceUuid);
            var aVOfSelectedVariable = answersVariables.Select(a => a.Variable.Id == request.VariableId).ToList();

            List<(Student Student, List<(Answer Answer, Variable Variable)>, double riskIndex)> results = [];
            foreach (var answerVArStudentId in answersVariables)
            {
                Student stud = await _studentRepository.GetByIdAsync(answerVArStudentId.StudentId);
                
            }
            return new ListResponse<(Student, List<(Answer, Variable)>?, double)>(0, []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating higher risk students: " + request);
            return new ListResponse<(Student, List<(Answer, Variable)>?, double)>(0, []);
        }
    }
}
