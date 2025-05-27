using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQueryHandler : IRequestHandler<GetPollsByCohortListQuery, List<Poll>>
    {
        private readonly IPollCohortRepository _repository;
        private readonly ILogger<GetPollsByCohortListQueryHandler> _logger;

        public GetPollsByCohortListQueryHandler( IPollCohortRepository PollCohortRepository, ILogger<GetPollsByCohortListQueryHandler> Logger)
        {
            _repository = PollCohortRepository;
            _logger = Logger;
        }

        public async Task<List<Poll>> Handle(GetPollsByCohortListQuery Request, CancellationToken CancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByCohortIdAsync(Request.CohortId);
            return listOfPolls;
        }

    }
}
