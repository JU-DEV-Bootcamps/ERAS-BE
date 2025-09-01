using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public sealed record GetRemissionByIdQuery() : IRequest<JURemission>
    {
        public required int Id;
    }
}