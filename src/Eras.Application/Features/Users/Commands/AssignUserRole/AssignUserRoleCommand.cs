using Eras.Domain.Enums;
using MediatR;

namespace Eras.Application.Features.Users.Commands.AssignUserRole;

public class AssignUserRoleCommand : IRequest<Unit>
{
    public required string TargetEmail { get; set; }
    public ErasRole Role { get; set; }
}