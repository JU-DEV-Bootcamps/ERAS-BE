using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Evaluations.Queries.GetAll;
using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Evaluations.Queries.GetByDateRange;

public class GetEvaluationsByDateRangeQueryHandler : IRequestHandler<GetEvaluationsByDateRangeQuery, List<Evaluation>>
{
    private readonly IEvaluationRepository _evaluationRepository;
    private readonly ILogger<GetEvaluationsByDateRangeQueryHandler> _logger;

    public GetEvaluationsByDateRangeQueryHandler(IEvaluationRepository EvaluationRepository, ILogger<GetEvaluationsByDateRangeQueryHandler> Logger)
    {
        _evaluationRepository = EvaluationRepository;
        _logger = Logger;
    }
    public async Task<List<Evaluation>> Handle(GetEvaluationsByDateRangeQuery Request, CancellationToken CancellationToken)
    {
        List<Evaluation> evaluations = await _evaluationRepository.GetByDateRange(
            Request.StartDate,
            Request.EndDate
        );

        return evaluations;
    }
}
