using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.Poll;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetAllByPollAndCohort
{
    public class GetAllByPollAndCohortQueryHandler
        : IRequestHandler<GetAllByPollAndCohortQuery, List<PollVariableDto>>
    {
        private readonly IPollCohortRepository _pollCohortRepository;
        private readonly ILogger<GetAllByPollAndCohortQueryHandler> _logger;

        public GetAllByPollAndCohortQueryHandler(
            IPollCohortRepository Repository,
            ILogger<GetAllByPollAndCohortQueryHandler> Logger
        )
        {
            _pollCohortRepository = Repository;
            _logger = Logger;
        }

        public async Task<List<PollVariableDto>> Handle(
            GetAllByPollAndCohortQuery Request,
            CancellationToken CancellationToken
        )
        {
            var result = await _pollCohortRepository.GetPollVariablesAsync(
                Request.pollId,
                Request.cohortId
            );

            return result;
        }
    }
}
