using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

public class GetStudentsByFiltersQueryHandler : 
    IRequestHandler<GetStudentsByFiltersQuery, PagedResult<StudentsByFiltersResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<GetStudentsByFiltersQueryHandler> _logger;

    public GetStudentsByFiltersQueryHandler(IErasEvaluationDetailsViewRepository Repository, IEvaluationRepository EvaluationRepository, ILogger<GetStudentsByFiltersQueryHandler> Logger)
    {
        _repository = Repository;
        _evaluationRepository = EvaluationRepository;
        _logger = Logger;
    }
    public async Task<PagedResult<StudentsByFiltersResponse>> Handle(GetStudentsByFiltersQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(Request.EvaluationId); 
            var startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc);

            var studentsList = await _repository.GetStudentsByFilters(
                Request.PollUuid,
                Request.ComponentNames, 
                Request.CohortIds,
                Request.VariableIds,
                Request.RiskLevels,
                Request.PageValues.Page, 
                Request.PageValues.PageSize,
                startDate,
                endDate
            );

            var totalCount = await _repository.CountStudentsByFilters(
                Request.PollUuid, Request.ComponentNames, Request.CohortIds,
                Request.VariableIds, Request.RiskLevels, startDate, endDate
            );

            var studentsResponses = studentsList.Select(Student => new StudentsByFiltersResponse
            {
                Id = Student.StudentId,
                Name = Student.StudentName,
                Email = Student.StudentEmail,
                AnswerId = Student.AnswerId,
                AnswerText = Student.AnswerText,
                RiskLevel = Student.RiskLevel
            }).ToList();

            return new PagedResult<StudentsByFiltersResponse>(totalCount, studentsResponses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering students: " + ex.Message);
            return new PagedResult<StudentsByFiltersResponse>(0, []);
        }
    }
}
