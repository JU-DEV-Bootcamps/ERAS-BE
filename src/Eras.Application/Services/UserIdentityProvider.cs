using Eras.Application.Contracts.Services;

namespace Eras.Application.Services;

public sealed class UserIdentityProvider : IUserIdentityProvider
{
    public const string UnknownUserId = "unknown";

    public string UserId { get; private set; } = UnknownUserId;

    public void SetUserId(string userId) => UserId = userId;
}
