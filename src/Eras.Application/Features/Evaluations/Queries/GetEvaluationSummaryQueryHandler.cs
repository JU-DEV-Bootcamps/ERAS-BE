using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries;

class GetEvaluationProcessSummaryQueryHandler(
    IEvaluationRepository EvaluationRepository,
    ILogger<GetEvaluationProcessSummaryQueryHandler> Logger
): IRequestHandler<GetEvaluationSummaryQuery, GetQueryResponse<Evaluation?>>
{
    private readonly IEvaluationRepository _evaluationRepository = EvaluationRepository;
    private readonly ILogger<GetEvaluationProcessSummaryQueryHandler> _logger = Logger;


    async Task<GetQueryResponse<Evaluation?>> IRequestHandler<GetEvaluationSummaryQuery, GetQueryResponse<Evaluation?>>.Handle(GetEvaluationSummaryQuery Request, CancellationToken CancellationToken)
    {
        _logger.LogDebug("Handling summarizing all evaluation processes");
        var evs = await _evaluationRepository.GetByIdAsync(Request.EvaluationId);
        return new GetQueryResponse<Evaluation?>(evs, evs == null ? "Evaluation not found" : $"Evaluation {evs.Status}", evs != null);
    }
}
