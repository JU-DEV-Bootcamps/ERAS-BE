using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;

public class GetStudentsByEvaluationIdQueryHandler : IRequestHandler<GetStudentsByEvaluationIdQuery, List<StudentsByFiltersResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<GetStudentsByEvaluationIdQueryHandler> _logger;

    public GetStudentsByEvaluationIdQueryHandler(IErasEvaluationDetailsViewRepository Repository, IEvaluationRepository EvaluationRepository, ILogger<GetStudentsByEvaluationIdQueryHandler> Logger)
    {
        _repository = Repository;
        _evaluationRepository = EvaluationRepository;
        _logger = Logger;
    }

    public async Task<List<StudentsByFiltersResponse>> Handle(GetStudentsByEvaluationIdQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            var evaluation = await _evaluationRepository.GetByIdAsync(Request.EvaluationId);
            var startDate = DateTime.SpecifyKind(evaluation.StartDate, DateTimeKind.Utc);
            var endDate = DateTime.SpecifyKind(evaluation.EndDate, DateTimeKind.Utc);

            var studentsList = await _repository.GetStudentsByEvaluationIdFilters(
                Request.EvaluationId,
                Request.ComponentNames,
                Request.CohortIds,
                Request.VariableIds,
                Request.RiskLevels,
                startDate,
                endDate
            );

            return studentsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering students by evaluation id: {Message}", ex.Message);
            return [];
        }
    }
}

