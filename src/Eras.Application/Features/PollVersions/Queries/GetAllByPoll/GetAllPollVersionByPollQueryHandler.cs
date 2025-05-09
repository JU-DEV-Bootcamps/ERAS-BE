using System;
using System.Collections.Generic;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollVersions.Queries.GetAllByPoll;
public class GetAllPollVersionByPollQueryHandler: IRequestHandler<GetAllPollVersionByPollQuery ,GetQueryResponse<List<PollVersion>>>
{
    private IPollVersionRepository _repository;
    private ILogger<GetAllPollVersionByPollQueryHandler> _logger;

    public GetAllPollVersionByPollQueryHandler(IPollVersionRepository Repository, 
        ILogger<GetAllPollVersionByPollQueryHandler> Logger)
    {
        this._repository = Repository;
        this._logger = Logger;
    }

    public async Task<GetQueryResponse<List<PollVersion>>> Handle(GetAllPollVersionByPollQuery Request, CancellationToken CancellationToken)
    {
        var query = await this._repository.GetAllByPollAsync(Request.PollId);
        return new GetQueryResponse<List<PollVersion>>(query, "Sucessfull query", true);
    }
}
