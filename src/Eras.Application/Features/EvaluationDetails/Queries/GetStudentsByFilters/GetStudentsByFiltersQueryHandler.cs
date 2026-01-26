using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetStudentsByFilters;

public class GetStudentsByFiltersQueryHandler : IRequestHandler<GetStudentsByFiltersQuery, List<StudentsByFiltersResponse>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly ILogger<GetStudentsByFiltersQueryHandler> _logger;

    public GetStudentsByFiltersQueryHandler(IErasEvaluationDetailsViewRepository Repository, ILogger<GetStudentsByFiltersQueryHandler> Logger)
    {
        _repository = Repository;
        _logger = Logger;
    }

    public async Task<List<StudentsByFiltersResponse>> Handle(GetStudentsByFiltersQuery Request, CancellationToken CancellationToken)
    {
        var studentsList = await _repository.GetStudentsByFilters(
            Request.PollId,
            Request.ComponentIds,
            Request.CohortIds,
            Request.VariableIds
        );

        return studentsList;
    }
}
