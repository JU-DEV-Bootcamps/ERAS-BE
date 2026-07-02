using System.Security.Claims;
using Eras.Application.Contracts.Infrastructure; 
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Eras.Infrastructure.Authorization;

public class ErasAdminHandler : AuthorizationHandler<ErasAdminRequirement>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ICurrentUserService _currentUserService;

    public ErasAdminHandler(
        IUserRoleRepository userRoleRepository, 
        ICurrentUserService currentUserService)
    {
        _userRoleRepository = userRoleRepository;
        _currentUserService = currentUserService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ErasAdminRequirement requirement)
    {
        var email = _currentUserService.Email;

        if (string.IsNullOrEmpty(email)) return;

        var userRole = await _userRoleRepository.GetByEmailAsync(email);

        if (userRole?.Role == ErasRole.ErasAdmin)
        {
            context.Succeed(requirement);
        }
    }
}