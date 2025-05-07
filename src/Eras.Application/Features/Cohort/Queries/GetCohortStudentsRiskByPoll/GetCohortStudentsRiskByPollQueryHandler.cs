using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public async Task<List<GetCohortStudentsRiskByPollResponse>> Handle(GetCohortStudentsRiskByPollQuery request, CancellationToken cancellationToken)
    {
        var listOfstudents = await _pollCohortRepository.GetCohortStudentsRiskByPoll(request.PollUuid, request.CohortId);
        return listOfstudents;
    }
}
