using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentByCohortPollQueryHandler(
    ICohortRepository cohortRepository,
    IStudentCohortRepository studentCohortRepository,
    IAnswerRepository answerRepository,
    IPollRepository pollRepository,
    ILogger<GetHigherRiskStudentByCohortPollQueryHandler> logger
  ) : IRequestHandler<GetHigherRiskStudentByCohortPollQuery, GetQueryResponse<List<(Student, List<Answer>?, double)>>>
{
    private readonly ICohortRepository _cohortRepository = cohortRepository;
    private readonly IStudentCohortRepository _studentCohortRepository = studentCohortRepository;
    private readonly IAnswerRepository _answerRepository = answerRepository;

    private readonly IPollRepository _pollRepository = pollRepository;
    private readonly ILogger<GetHigherRiskStudentByCohortPollQueryHandler> _logger = logger;
    public int DefaultTakeNumber = 5;

    public async Task<GetQueryResponse<List<(Student, List<Answer>?, double)>>> Handle(GetHigherRiskStudentByCohortPollQuery request, CancellationToken cancellationToken)
    {
        try
        {
            int TakeNStudents = request.Take.HasValue && request.Take.Value > 0 ? request.Take.Value : DefaultTakeNumber;
            var poll = await _pollRepository.GetByNameAsync(request.PollName) ?? throw new KeyNotFoundException("Poll not found");

            var cohort = request.CohortName != null ? await _cohortRepository.GetByNameAsync(request.CohortName) : null;
            var cohortStudents = (
                cohort != null
                    ? await _studentCohortRepository.GetAllStudentsByCohortIdAsync(cohort.Id)
                    : null
                ) ?? throw new KeyNotFoundException("No students found for the cohort");

            List<(Student Student, List<Answer>? Answers, double RiskIndex)> studentsAnswers = [];
            foreach (var student in cohortStudents)
            {
                var answers = await _answerRepository.GetByStudentIdAsync(student.Uuid);
                //Higher risk index calculator
                double riskIndex = 0;
                if (answers?.Count > 0)
                {
                    riskIndex = answers.Average(a => a.RiskLevel);
                }
                studentsAnswers.Add((student, answers, riskIndex));
            }
            var topN = studentsAnswers.OrderByDescending(s => s.RiskIndex).Take(TakeNStudents).ToList();
            return new GetQueryResponse<List<(Student Student, List<Answer>? Answers, double RiskIndex)>>(topN, "successful", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error: {ex.Message} occurred while calculating higher risk students: " + request);
            return new GetQueryResponse<List<(Student Student, List<Answer>? Answers, double RiskIndex)>>([], $"Failed to retrieve top risk students. Error: {ex.Message}", false);
        }
    }
}
