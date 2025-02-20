using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQueryHandler(
    ICohortRepository cohortRepository,
    IStudentCohortRepository studentCohortRepository,
    IAnswerRepository answerRepository,
    IPollRepository pollRepository,
    ILogger<GetHigherRiskStudentQueryHandler> logger
  ) : IRequestHandler<GetHigherRiskStudentQuery, ListResponse<(Student, List<Answer>?, double)>>
{
    private readonly ICohortRepository _cohortRepository = cohortRepository;
    private readonly IStudentCohortRepository _studentCohortRepository = studentCohortRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;

    private readonly IPollRepository _pollRepository = pollRepository;
    private readonly ILogger<GetHigherRiskStudentQueryHandler> _logger = logger;
    public int DefaultTakeNumber = 5;

    public async Task<ListResponse<(Student, List<Answer>?, double)>> Handle(GetHigherRiskStudentQuery request, CancellationToken cancellationToken)
    {
        try {
            int TakeNStudents = request.Take ?? DefaultTakeNumber;
            //TODO: Should it be a pollInstance or is it okay to be a poll? User (Service students) may only have access to the poll name??.
            var poll = await _pollRepository.GetByNameAsync(request.PollNameCosmicLatte) ?? throw new KeyNotFoundException("Poll not found");

            var cohort = request.CohortName != null ? await _cohortRepository.GetByNameAsync(request.CohortName) : null;
            var cohortStudents = (
                cohort != null
                    ? await _studentCohortRepository.GetAllStudentsByCohortIdAsync(cohort.Id)
                    : null
                ) ?? throw new KeyNotFoundException("No students found for the cohort");
            List<(Student, List<Answer>?, double riskIndex)> studentsAnswers = [];
            foreach (var student in cohortStudents){
                var answers = await _answerRepository.GetByPollInstanceIdAsync(poll.Uuid);
                //Expensive higher risk index calculator
                double riskIndex = 0;
                if(answers?.Count > 0) {
                    riskIndex = answers.Average(a => a.RiskLevel);
                }
                studentsAnswers.Add((student, answers, riskIndex));
            }

            List<(Student, List<Answer>?, double riskIndex)> topRiskStudents = [];
            topRiskStudents = [.. studentsAnswers.OrderByDescending(s => s.riskIndex).Take(TakeNStudents)];

            return new ListResponse<(Student, List<Answer>?, double)>(
                TakeNStudents,
                topRiskStudents
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating higher risk students: " + request);
            return new ListResponse<(Student, List<Answer>?, double)>(0, []);
        }
    }
}
