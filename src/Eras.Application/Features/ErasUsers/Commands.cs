using Eras.Application.DTOs.UsersManagement;

using MediatR;

namespace Eras.Application.Features.ErasUsers;

public sealed record CreateErasUserCommand(ErasUserDTO ErasUser) : IRequest<ErasUserDTO>;
public sealed record UpdateErasUserCommand(ErasUserDTO ErasUser) : IRequest<ErasUserDTO>;

/// <summary>
/// Self-service update of the authenticated caller's own editable profile fields.
/// Resolves the target user the same way GetMyProfileQuery does (Sub, falling back
/// to Email). Does not touch first name, last name, email, or role — those stay
/// exclusively Keycloak-sync-owned.
/// </summary>
public sealed record UpdateUserProfileCommand(
    string? Sub,
    string Email,
    string? EmployeeId,
    string? Department,
    string? Phone,
    string? Position,
    string? About
) : IRequest<ErasUserDTO>;

/// <summary>
/// Upserts the local ERAS user profile from the identity's Keycloak claims.
/// Unlike Create/Update, this is idempotent: it creates the user if missing,
/// updates it only if not yet synced, and is a no-op otherwise.
/// </summary>
public sealed record SyncErasUserCommand(
    string? Sub,
    string Email,
    string FirstName,
    string LastName,
    string Role
) : IRequest<ErasUserDTO>;