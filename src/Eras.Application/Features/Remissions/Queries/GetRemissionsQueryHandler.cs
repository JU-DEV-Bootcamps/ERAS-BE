using Eras.Application.Models.Response.Controllers.RemissionsController;
using Eras.Application.Utils;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public class GetRemissionsQueryHandler
        : IRequestHandler<GetRemissionsQuery, PagedResult<GetRemissionsQueryResponse>>
    {
        private readonly IRemissionRepository _remissionRepository;
        private readonly ILogger<GetRemissionQueryHandler> _logger;
        }
    public GetRemissionsQueryHandler(IRemissionRepository RemissionRepository, ILogger<GetRemissionsQueryResponse> Logger)
        {
            _remissionRepository = RemissionRepository;
            _logger = Logger;
        }

        public async Task<List<GetRemissionsQuery>> Handle(GetRemissionsQuery Request, CancellationToken CancellationToken)
        {
            try
            {
                var polls = await _remissionRepository.GetAllAsync();
                var pollsList = polls.ToList();
                var pollsResponses = pollsList.Select(Poll => new GetRemissionsQueryResponse
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