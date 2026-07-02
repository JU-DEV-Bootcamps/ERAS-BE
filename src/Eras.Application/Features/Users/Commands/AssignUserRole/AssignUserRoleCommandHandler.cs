using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Users.Commands.AssignUserRole;

public class AssignUserRoleCommandHandler : IRequestHandler<AssignUserRoleCommand, Unit>
{
    private readonly IUserRoleRepository _userRoleRepository;

    public AssignUserRoleCommandHandler(IUserRoleRepository UserRoleRepository)
    {
        _userRoleRepository = UserRoleRepository;
    }

    public async Task<Unit> Handle(AssignUserRoleCommand Request, CancellationToken CancellationToken)
    {
        var existing = await _userRoleRepository.GetByEmailAsync(Request.TargetEmail);

        if (existing is null)
        {
            await _userRoleRepository.AddAsync(new UserRoleEntity
            {
                Email = Request.TargetEmail,
                Role = Request.Role,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Role = Request.Role;
            existing.UpdatedAt = DateTime.UtcNow;
            await _userRoleRepository.UpdateAsync(existing);
        }
        return Unit.Value;
    }
}