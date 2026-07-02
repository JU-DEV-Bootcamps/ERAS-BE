using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Users;
using Eras.Domain.Enums;
using MediatR;

namespace Eras.Application.Features.Users.Queries.GetCurrentUserRole;

public class GetCurrentUserRoleQueryHandler : IRequestHandler<GetCurrentUserRoleQuery, GetCurrentUserRoleResponse>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserRoleQueryHandler(IUserRoleRepository UserRoleRepository, ICurrentUserService CurrentUserService)
    {
        _userRoleRepository = UserRoleRepository;
        _currentUserService = CurrentUserService;
    }

    public async Task<GetCurrentUserRoleResponse> Handle(GetCurrentUserRoleQuery request, CancellationToken cancellationToken)
    {
        var email = _currentUserService.Email;

        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAccessException();

        var userRole = await _userRoleRepository.GetByEmailAsync(email);

        return new GetCurrentUserRoleResponse
        {
            Id = _currentUserService.KeycloakSub ?? string.Empty,
            Email = email,
            FirstName = _currentUserService.FirstName,
            LastName = _currentUserService.LastName,
            Role = userRole?.Role ?? ErasRole.User
        };
    }
}