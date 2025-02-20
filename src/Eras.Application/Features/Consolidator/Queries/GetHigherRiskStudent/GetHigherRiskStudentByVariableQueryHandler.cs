using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentByVariableQueryHandler(
    ICohortRepository cohortRepository,
    IStudentCohortRepository studentCohortRepository,
    IAnswerRepository answerRepository,
    IPollVariableRepository pollVariableRepository,
    IVariableRepository variableRepository,
    IPollInstanceRepository pollInstanceRepository,
    ILogger<GetHigherRiskStudentByVariableQueryHandler> logger
  ) : IRequestHandler<GetHigherRiskStudentByVariableQuery, ListResponse<(Student, List<Answer>?, double)>>
{
    private readonly ICohortRepository _cohortRepository = cohortRepository;
    private readonly IStudentCohortRepository _studentCohortRepository = studentCohortRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;

    private readonly IVariableRepository _variableRepository = variableRepository;
    private readonly IPollVariableRepository _pollVariableRepository = pollVariableRepository;
    private readonly IPollInstanceRepository _pollInstanceRepository = pollInstanceRepository;
    private readonly ILogger<GetHigherRiskStudentByVariableQueryHandler> _logger = logger;
    public int DefaultTakeNumber = 5;

    public async Task<ListResponse<(Student, List<Answer>?, double)>> Handle(GetHigherRiskStudentByVariableQuery request, CancellationToken cancellationToken)
    {
        try {
            int TakeNStudents = request.Take ?? DefaultTakeNumber;
            //WIP
            //var variable = _pollVariableRepository.GetByPollIdAndVariableIdAsync(request.VariableId);
            var pollInstance = await _pollInstanceRepository.GetByUuidAsync(request.PollInstanceUuid);
            //var variable = _variableRepository.GetByIdAsync(request.VariableUuid);
            return new ListResponse<(Student, List<Answer>?, double)>(0, []);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating higher risk students: " + request);
            return new ListResponse<(Student, List<Answer>?, double)>(0, []);
        }
    }
}
