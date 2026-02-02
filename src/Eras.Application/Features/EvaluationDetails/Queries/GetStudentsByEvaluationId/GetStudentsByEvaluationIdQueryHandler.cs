using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByEvaluationId;

public class GetStudentsByEvaluationIdQueryHandler : IRequestHandler<GetStudentsByEvaluationIdQuery, List<StudentsByFiltersResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly ILogger<GetStudentsByEvaluationIdQueryHandler> _logger;

    public GetStudentsByEvaluationIdQueryHandler(IErasEvaluationDetailsViewRepository Repository, ILogger<GetStudentsByEvaluationIdQueryHandler> Logger)
    {
        _repository = Repository;
        _logger = Logger;
    }

    public async Task<List<StudentsByFiltersResponse>> Handle(GetStudentsByEvaluationIdQuery Request, CancellationToken CancellationToken)
    {
        var studentsList = await _repository.GetStudentsByEvaluationIdFilters(
            Request.EvaluationId,
            Request.ComponentNames,
            Request.CohortIds,
            Request.VariableIds,
            Request.RiskLevels
        );

        return studentsList;
    }
}
