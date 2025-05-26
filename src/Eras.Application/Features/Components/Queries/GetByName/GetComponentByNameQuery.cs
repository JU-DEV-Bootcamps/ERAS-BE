using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Components.Queries.GetByName;
public class GetComponentByNameQuery: IRequest<GetQueryResponse<Component>>
{
    public required string componentName { get; set; }
}
