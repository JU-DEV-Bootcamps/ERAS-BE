using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using Eras.Application.Models.Enums;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Queries.GetPollByName;
public class GetPollByNameQueryHandler: IRequestHandler<GetPollByNameQuery, GetQueryResponse<Poll>>
{
    private IPollRepository _pollRepository;
    private ILogger<GetPollByNameQuery> _logger;

    public GetPollByNameQueryHandler(
        IPollRepository PollsRepository, 
        ILogger<GetPollByNameQuery> Logger
    )
    {
        _pollRepository = PollsRepository;
        _logger = Logger;
    }

    public async Task<GetQueryResponse<Poll>> Handle(GetPollByNameQuery Request, CancellationToken CancellationToken) {
        Poll? poll = await _pollRepository.GetByNameAsync(Request.pollName);
        if (poll == null) return new GetQueryResponse<Poll>(new Poll(),"Poll Not Found",true,
            QueryEnums.QueryResultStatus.NotFound);
        return new GetQueryResponse<Poll>(poll,"Poll Found", true);
    }
}
