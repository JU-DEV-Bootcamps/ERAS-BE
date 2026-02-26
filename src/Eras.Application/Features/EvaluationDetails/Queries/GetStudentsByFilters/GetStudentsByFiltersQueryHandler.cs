using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

public class GetStudentsByFiltersQueryHandler : 
    IRequestHandler<GetStudentsByFiltersQuery, PagedResult<StudentsByFiltersResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly ILogger<GetStudentsByFiltersQueryHandler> _logger;

    public GetStudentsByFiltersQueryHandler(IErasEvaluationDetailsViewRepository Repository, ILogger<GetStudentsByFiltersQueryHandler> Logger)
    {
        _repository = Repository;
        _logger = Logger;
    }
    public async Task<PagedResult<StudentsByFiltersResponse>> Handle(GetStudentsByFiltersQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var studentsList = await _repository.GetStudentsByFilters(
                Request.PollUuid,
                Request.ComponentNames, 
                Request.CohortIds,
                Request.VariableIds,
                Request.RiskLevels,
                Request.PageValues.Page, 
                Request.PageValues.PageSize
            );

            var totalCount = await _repository.CountStudentsByFilters(
                Request.PollUuid, Request.ComponentNames, Request.CohortIds,
                Request.VariableIds, Request.RiskLevels
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
