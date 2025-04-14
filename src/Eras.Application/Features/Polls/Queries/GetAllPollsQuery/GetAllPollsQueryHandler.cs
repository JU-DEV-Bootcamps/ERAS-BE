using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery;

public class GetAllPollsQueryHandler : IRequestHandler<GetAllPollsQuery, List<Poll>>
{
    private readonly IPollRepository _pollRepository;
    private readonly ILogger<GetAllPollsQuery> _logger;

    public GetAllPollsQueryHandler(
        IPollRepository PollRepository,
        ILogger<GetAllPollsQuery> Logger
    )
    {
        _pollRepository = PollRepository;
        _logger = Logger;
    }

    public async Task<List<Poll>> Handle(GetAllPollsQuery Request, CancellationToken CancellationToken)
    {
        try
        {
            return [.. _pollRepository.GetAllAsync().Result];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting polls");
            return [];
        }
    }
}
