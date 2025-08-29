using Eras.Application.Models.Response.Controllers.RemissionsController;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Remmisions.Queries.GetRemissions
{
    public sealed record GetRemissionsQuery() : IRequest<List<JURemission>>;
}