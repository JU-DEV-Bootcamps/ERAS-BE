using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Queries;

public class GetMyProfileQueryHandlerTests
{
    private readonly Mock<IErasUsersRepository> _repositoryMock = new();
    private readonly GetMyProfileQueryHandler _handler;

    public GetMyProfileQueryHandlerTests()
    {
        _handler = new GetMyProfileQueryHandler(_repositoryMock.Object);
    }

    private static ErasUserDTO CreateProfile() => new()
    {
        Id = 1,
        Sub = "sub-123",
        Email = "user@test.com",
        FirstName = "First",
        LastName = "Last",
        Role = ErasRole.Officer.Label,
        EmployeeId = "E-1",
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    public async Task Handle_ResolvesBySub_WhenSubIsProvided()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        Assert.Same(profile, result);
        _repositoryMock.Verify(R => R.GetErasUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FallsBackToEmail_WhenSubLookupMisses()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        Assert.Same(profile, result);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None));
    }
}
