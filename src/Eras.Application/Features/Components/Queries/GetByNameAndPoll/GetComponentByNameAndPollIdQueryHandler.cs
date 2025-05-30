using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Components.Queries.GetByNameAndPoll;
public class GetComponentByNameAndPollIdQueryHandler: IRequestHandler<GetComponentByNameAndPollIdQuery, GetQueryResponse<Component>>
{
    private IComponentRepository _componentRepository;
    private ILogger<GetComponentByNameAndPollIdQueryHandler> _logger;

    public GetComponentByNameAndPollIdQueryHandler(IComponentRepository ComponentRepository, ILogger<GetComponentByNameAndPollIdQueryHandler> Logger)
    {
        _componentRepository = ComponentRepository;
        _logger = Logger;
    }
    public async Task<GetQueryResponse<Component>> Handle(GetComponentByNameAndPollIdQuery Request, CancellationToken CancellationToken)
    {

        var response = await _componentRepository.GetByNameAndPollIdAsync(Request.ComponentName, Request.PollId);
        if (response == null)
        {
            return new GetQueryResponse<Component>(new Component(), "Component Found0", true, QueryEnums.QueryResultStatus.NotFound);
        }
        return new GetQueryResponse<Component>(response, "Component Found0", true, QueryEnums.QueryResultStatus.Success);
    }
}
