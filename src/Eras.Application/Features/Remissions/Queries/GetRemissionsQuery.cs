using Eras.Application.Models.Response.Controllers.RemissionsController;
using Eras.Application.Utils;

using MediatR;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public sealed record GetRemissionsQuery(Pagination Query) : IRequest<PagedResult<GetRemissionsQueryResponse>>;
}