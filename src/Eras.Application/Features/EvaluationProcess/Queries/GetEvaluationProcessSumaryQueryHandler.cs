using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Cohort.Queries.GetCohortsList;
using Eras.Application.Features.Cohort.Queries;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.EvaluationProcess.Queries
{
    class GetEvaluationProcessSumaryQueryHandler(IPollCohortRepository repository, ILogger<GetEvaluationProcessSummaryQuery> logger) : IRequestHandler<GetEvaluationProcessSummaryQuery, List<Poll>>
    {
        public Task<List<Poll>> Handle(GetEvaluationProcessSummaryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
