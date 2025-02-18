using System;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Consolidator.Queries.GetHigherRiskStudent;

public class GetHigherRiskStudentQueryHandler: IRequestHandler<GetHigherRiskStudentQuery, BaseResponse>
{
    private readonly ICohortRepository _cohortRepository;
    private readonly IAnswerRepository _answerRepository;
    private readonly ILogger<GetHigherRiskStudentQueryHandler> _logger;

    public GetHigherRiskStudentQueryHandler(ICohortRepository cohortRepository, IAnswerRepository answerRepository, ILogger<GetHigherRiskStudentQueryHandler> logger)
    {
        _cohortRepository = cohortRepository;
        _answerRepository = answerRepository;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(GetHigherRiskStudentQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var defaultTakeNumber = 5;
            var cohort = await _cohortRepository.GetByNameAsync(request.CohortId);
            if(cohort == null) {
                return new BaseResponse($"The cohort {request.CohortId} does not exist", false);
            }
            List<(int Risk, Student student)> studentsRisk = [];
            foreach (var student in cohort.Students)
            {
                //TODO: Add method to repo to get answers by students in a cohort context
                //_answerRepository.GetByStudentId(student.Id);
                //StudentAnswers.SumRiskLevel();
                studentsRisk.Add((5, student));
            }
            var topHigherRisk = studentsRisk.OrderByDescending(s => s.Risk).Take(request.TakeNumber | defaultTakeNumber);
            return new BaseResponse("The students with higher risk are {topHigherRisk.Format()}", true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while calculating higher risk students: " + request);
            return new BaseResponse(false);
        }
    }
}
