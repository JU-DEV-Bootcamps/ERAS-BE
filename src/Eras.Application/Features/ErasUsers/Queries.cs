using Eras.Application.DTOs.UsersManagement;

using MediatR;

namespace Eras.Application.Features.ErasUsers;

public sealed record GetErasUserCheckByEmailQuery(string Email) : IRequest<ErasUserDTO?>;

/// <summary>
/// Lists real ERAS users (synced from Keycloak), optionally filtered by role — e.g. to populate
/// an "assigned professional" picker with actual accounts instead of a disconnected name catalog,
/// or (with no role) to resolve any user's sub back to a display name.
/// </summary>
public sealed record GetErasUsersByRoleQuery(string? Role = null) : IRequest<IEnumerable<ErasUserDTO>>;

/// <summary>
/// Resolves the authenticated caller's own ERAS user profile, preferring the
/// Keycloak "sub" claim and falling back to email, matching SyncErasUserCommand's
/// resolution order.
/// </summary>
public sealed record GetMyProfileQuery(string? Sub, string Email) : IRequest<MyProfileDTO>;