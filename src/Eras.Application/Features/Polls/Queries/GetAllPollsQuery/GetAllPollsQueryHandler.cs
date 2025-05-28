using Eras.Application.Contracts.Persistence;
using Eras.Application.Models;
using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Application.Models.Response.Controllers.StudentsController;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery
{
    public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, List<GetPollsQueryResponse>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<GetAllPollsQuery> _logger;

    public GetAllPollsQueryHandler(IPollRepository PollRepository, ILogger<GetAllPollsQuery> Logger)
    {
        _pollRepository = PollRepository;
        _logger = Logger;
    }

        public async Task<List<GetPollsQueryResponse>> Handle(GetAllPollsQuery Request, CancellationToken CancellationToke)
        {
            try
            {
                var polls = await _pollRepository.GetAllAsync();
                var pollsList = polls.ToList();
                var pollsResponses = pollsList.Select(Poll => new GetPollsQueryResponse
                {
                    Id = Poll.Id,
                    Uuid = Poll.Uuid,
                    Name = Poll.Name,
                    LastVersion = Poll.LastVersion,
                    LastVersionDate = Poll.LastVersionDate,
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
