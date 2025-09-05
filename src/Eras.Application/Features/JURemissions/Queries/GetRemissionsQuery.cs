using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public sealed record GetRemissionsQuery() : IRequest<PagedResult<JURemission>>
    {
        public Pagination Query { get; set; } = new Pagination();
    }
}