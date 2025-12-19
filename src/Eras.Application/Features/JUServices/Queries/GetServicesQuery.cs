using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.JUServices.Queries.GetJUServices;
public class GetJUServicesQuery() : IRequest<PagedResult<JUService>>
{
    public Pagination Query { get; set; } = new Pagination();
}