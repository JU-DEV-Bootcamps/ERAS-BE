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
            IPollCohortRepository repository,
            ILogger<GetAllByPollAndCohortQueryHandler> logger
        )
        {
            _pollCohortRepository = repository;
            _logger = logger;
        }

        public async Task<List<PollVariableDto>> Handle(
            GetAllByPollAndCohortQuery request,
            CancellationToken cancellationToken
        )
        {
            var result = await _pollCohortRepository.GetPollVariablesAsync(
                request.pollId,
                request.cohortId
            );

            return result;
        }
    }
}
