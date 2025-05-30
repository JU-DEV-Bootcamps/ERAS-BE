using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQueryHandler : IRequestHandler<GetPollsByCohortListQuery, List<GetPollsQueryResponse>>
    {
        private readonly IPollCohortRepository _repository;
        private readonly ILogger<GetPollsByCohortListQueryHandler> _logger;

        public GetPollsByCohortListQueryHandler( IPollCohortRepository PollCohortRepository, ILogger<GetPollsByCohortListQueryHandler> Logger)
        {
            _repository = PollCohortRepository;
            _logger = Logger;
        }

        public async Task<List<GetPollsQueryResponse>> Handle(GetPollsByCohortListQuery Request, CancellationToken CancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByCohortIdAsync(Request.CohortId);
            var pollsResponses = listOfPolls.Select(Poll => new GetPollsQueryResponse
            {
                Id = Poll.Id,
                Uuid = Poll.Uuid,
                Name = Poll.Name,
                LastVersion = Poll.LastVersion,
                LastVersionDate = Poll.LastVersionDate,
            }).ToList();

            return pollsResponses;
        }

    }
}
