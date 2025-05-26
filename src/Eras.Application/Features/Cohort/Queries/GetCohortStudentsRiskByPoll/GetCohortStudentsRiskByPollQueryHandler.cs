using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Calculations;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Cohort.Queries.GetCohortStudentsRiskByPoll;
public class GetCohortStudentsRiskByPollQueryHandler : IRequestHandler<GetCohortStudentsRiskByPollQuery, List<GetCohortStudentsRiskByPollResponse>>
{
    private readonly IPollCohortRepository _pollCohortRepository;
    private readonly ILogger<GetCohortStudentsRiskByPollQueryHandler> _logger;
    public GetCohortStudentsRiskByPollQueryHandler(IPollCohortRepository PollCohortRepository, ILogger<GetCohortStudentsRiskByPollQueryHandler> Logger)
    {
        _pollCohortRepository = PollCohortRepository;
        _logger = Logger;
    }

    public async Task<List<GetCohortStudentsRiskByPollResponse>> Handle(GetCohortStudentsRiskByPollQuery Request, CancellationToken CancellationToken)
    {
        var listOfstudents = await _pollCohortRepository.GetCohortStudentsRiskByPoll(Request.PollUuid, Request.CohortId);
        return listOfstudents;
    }
}
