using Eras.Application.Utils;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public sealed record GetRemissionByIdQuery(Pagination Query) : IRequest<JURemission>
    {
        public required int Id;
    }
}