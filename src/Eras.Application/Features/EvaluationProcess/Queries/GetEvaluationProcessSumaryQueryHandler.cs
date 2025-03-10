using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationProcess.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(IPollCohortRepository repository, ILogger<GetEvaluationProcessSummaryQuery> logger) : IRequestHandler<GetEvaluationProcessSummaryQuery, List<Poll>>
    {
        public async Task<List<Poll>> Handle(GetEvaluationProcessSummaryQuery request, CancellationToken cancellationToken)
        {
            var polls = await repository.GetAllAsync();
            return [.. polls];
        }
    }
}
