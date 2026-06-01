using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapByPollIdAndVariableIds;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationDetails.Queries.GetEvaluationDetailsByFilters;

public class GetEvaluationDetailsByFiltersQueryHandler : IRequestHandler<GetEvaluationDetailsByFiltersQuery, List<ErasEvaluationDetailsView>>
{
    private readonly IErasEvaluationDetailsViewRepository _repository;
    private readonly ILogger<GetEvaluationDetailsByFiltersQueryHandler> _logger;

    public GetEvaluationDetailsByFiltersQueryHandler(IErasEvaluationDetailsViewRepository Repository, ILogger<GetEvaluationDetailsByFiltersQueryHandler> Logger)
    {
        _repository = Repository;
        _logger = Logger;
    }

    public async Task<List<ErasEvaluationDetailsView>> Handle(GetEvaluationDetailsByFiltersQuery Request, CancellationToken CancellationToken)
    {
        var evaluationDetails = await _repository.GetByFiltersAsync(
            Request.PollId,
            Request.ComponentIds,
            Request.CohortIds,
            Request.VariableIds
        );

        return evaluationDetails;
    }
}
