using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortComponentsByPoll;
public class GetCohortComponentsByPollQueryHandler : IRequestHandler<GetCohortComponentsByPollQuery, List<GetCohortComponentsByPollResponse>>
{
    private readonly IPollCohortRepository _pollCohortRepository;
    private readonly ILogger<GetCohortComponentsByPollQueryHandler> _logger;
    public GetCohortComponentsByPollQueryHandler(IPollCohortRepository PollCohortRepository, ILogger<GetCohortComponentsByPollQueryHandler> Logger)
    {
        _pollCohortRepository = PollCohortRepository;
        _logger = Logger;
    }

    public async Task<List<GetCohortComponentsByPollResponse>> Handle(GetCohortComponentsByPollQuery Request, CancellationToken CancellationToken)
    {
        var listCohortComponentsByPoll = await _pollCohortRepository.GetCohortComponentsByPoll(Request.PollUuid);
        return listCohortComponentsByPoll;
    }
}
