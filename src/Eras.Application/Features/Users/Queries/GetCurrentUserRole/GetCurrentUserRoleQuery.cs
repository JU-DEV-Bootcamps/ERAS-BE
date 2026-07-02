using Eras.Application.Models.Response.Users;
using MediatR;

namespace Eras.Application.Features.Users.Queries.GetCurrentUserRole;

public class GetCurrentUserRoleQuery : IRequest<GetCurrentUserRoleResponse>
{
}