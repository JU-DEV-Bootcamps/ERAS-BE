using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public GetPollsByCohortListQueryHandler( IPollCohortRepository pollCohortRepository, ILogger<GetPollsByCohortListQueryHandler> logger)
        {
            _repository = pollCohortRepository;
            _logger = logger;
        }

        public async Task<List<GetPollsQueryResponse>> Handle(GetPollsByCohortListQuery request, CancellationToken cancellationToken)
        {
            var listOfPolls = await _repository.GetPollsByCohortIdAsync(request.CohortId);
            var pollsResponses = listOfPolls.Select(poll => new GetPollsQueryResponse
            {
                Id = poll.Id,
                Uuid = poll.Uuid,
                Name = poll.Name,
                LastVersion = poll.LastVersion,
                LastVersionDate = poll.LastVersionDate,
            }).ToList();

            return pollsResponses;
        }

    }
}
