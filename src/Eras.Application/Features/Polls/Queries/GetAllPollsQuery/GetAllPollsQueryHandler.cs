using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery
{
    public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, List<GetPollsQueryResponse>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<GetAllPollsQuery> _logger;

        public GetAllPollsQueryHandler(
            IPollRepository pollRepository,
            ILogger<GetAllPollsQuery> logger
        )
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<List<GetPollsQueryResponse>> Handle(GetAllPollsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var polls = _pollRepository.GetAllAsync().Result.ToList();
                var pollsResponses = polls.Select(poll => new GetPollsQueryResponse
                {
                    Id = poll.Id,
                    Uuid = poll.Uuid,
                    Name = poll.Name,
                    LastVersion = poll.LastVersion,
                    LastVersionDate = poll.LastVersionDate,
                }).ToList();

                return pollsResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting polls: " + ex.Message);
                return new List<GetPollsQueryResponse>();
            }
        }
    }
}
