using Eras.Application.Services;

namespace Eras.Application.Tests.Services;

public class UserIdentityProviderTest
{
    [Fact]
    public void UserId_Should_DefaultToUnknown_When_NeverSet()
    {
        var provider = new UserIdentityProvider();

        Assert.Equal(UserIdentityProvider.UnknownUserId, provider.UserId);
    }

    [Fact]
    public void SetUserId_Should_UpdateUserId()
    {
        var provider = new UserIdentityProvider();

        provider.SetUserId("user-1");

        Assert.Equal("user-1", provider.UserId);
    }

    [Fact]
    public void SetUserId_Should_OverwritePreviousValue_When_CalledAgain()
    {
        var provider = new UserIdentityProvider();
        provider.SetUserId("user-1");

        provider.SetUserId("user-2");

        Assert.Equal("user-2", provider.UserId);
    }
}
