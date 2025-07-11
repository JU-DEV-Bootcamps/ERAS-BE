﻿using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.Students;

public class GetStudentTopQueryHandler(
    ICohortRepository CohortRepository,
    IStudentCohortRepository StudentCohortRepository,
    IAnswerRepository AnswerRepository,
    IPollRepository PollRepository,
    ILogger<GetStudentTopQueryHandler> Logger
  ) : IRequestHandler<GetStudentTopQuery, GetQueryResponse<List<(Student, List<Answer>, decimal)>>>
{
    private readonly ICohortRepository _cohortRepository = CohortRepository;
    private readonly IStudentCohortRepository _studentCohortRepository = StudentCohortRepository;
    private readonly IAnswerRepository _answerRepository = AnswerRepository;

    private readonly IPollRepository _pollRepository = PollRepository;
    private readonly ILogger<GetStudentTopQueryHandler> _logger = Logger;
    public int DefaultTakeNumber = 5;

    public async Task<GetQueryResponse<List<(Student, List<Answer>, decimal)>>> Handle(GetStudentTopQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var TakeNStudents = Request.Take.HasValue && Request.Take.Value > 0 ? Request.Take.Value : DefaultTakeNumber;
            Poll poll = await _pollRepository.GetByNameAsync(Request.PollName) ?? throw new KeyNotFoundException("Poll not found");

            Cohort? cohort = Request.CohortName != null ? await _cohortRepository.GetByNameAsync(Request.CohortName) : null;
            IEnumerable<Student> cohortStudents = (
                cohort != null
                    ? await _studentCohortRepository.GetAllStudentsByCohortIdAsync(cohort.Id)
                    : null
                ) ?? throw new KeyNotFoundException("No students found for the cohort");

            List<(Student Student, List<Answer> Answers, decimal RiskIndex)> studentsAnswers = [];
            foreach (var student in cohortStudents)
            {
                List<Answer> answers = await _answerRepository.GetByStudentIdAsync(student.Uuid);
                //Higher risk index calculator
                decimal riskIndex = 0;
                if (answers.Count > 0)
                {
                    riskIndex = answers.Average(A => A.RiskLevel);
                }
                //If the student has not answered the poll, we will not include them in the list.
                if (answers.Count == 0)
                {
                    continue;
                }
                studentsAnswers.Add((student, answers, riskIndex));
            }
            var topN = studentsAnswers.OrderByDescending(S => S.RiskIndex).Take(TakeNStudents).ToList();
            return new GetQueryResponse<List<(Student Student, List<Answer> Answers, decimal RiskIndex)>>(topN, "successful", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating higher risk students");
            return new GetQueryResponse<List<(Student Student, List<Answer> Answers, decimal RiskIndex)>>([], $"Failed to retrieve top risk students. Error: {ex.Message}", false);
        }
    }
}
