
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Components.Queries.GetByName;
public class GetComponentBynameQueryHandler: IRequestHandler<GetComponentByNameQuery, GetQueryResponse<Component>>
{
    private IComponentRepository _componentRepository;
    private ILogger<GetComponentBynameQueryHandler> _logger;

    public GetComponentBynameQueryHandler(IComponentRepository ComponentRepository, ILogger<GetComponentBynameQueryHandler> Logger)
    {
        _componentRepository = ComponentRepository;
        _logger = Logger;
    }
    public async Task<GetQueryResponse<Component>> Handle(GetComponentByNameQuery Request, CancellationToken CancellationToken)
    {

        var response = await _componentRepository.GetByNameAsync(Request.componentName);
        if (response == null) 
        {
            return new GetQueryResponse<Component>(new Component(), "Component Found0", true,QueryEnums.QueryResultStatus.NotFound);
        }
        return new GetQueryResponse<Component>(response, "Component Found0", true, QueryEnums.QueryResultStatus.Success);
    }
}
