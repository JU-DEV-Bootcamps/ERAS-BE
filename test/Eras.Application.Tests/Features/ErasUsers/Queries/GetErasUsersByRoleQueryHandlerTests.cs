using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Queries;

public class GetErasUsersByRoleQueryHandlerTests
{
    private readonly Mock<IErasUsersRepository> _repositoryMock = new();
    private readonly GetErasUsersByRoleQueryHandler _handler;

    public GetErasUsersByRoleQueryHandlerTests()
    {
        _handler = new GetErasUsersByRoleQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUsersForTheGivenRole()
    {
        var professionals = new List<ErasUserDTO>
        {
            new()
            {
                Sub = "sub-1",
                Email = "a@test.com",
                FirstName = "A",
                LastName = "One",
                Role = ErasRole.Professional.Label,
                Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            },
        };

        _repositoryMock
            .Setup(R => R.GetErasUsersByRoleAsync(ErasRole.Professional.Label))
            .ReturnsAsync(professionals);

        var result = await _handler.Handle(
            new GetErasUsersByRoleQuery(ErasRole.Professional.Label), CancellationToken.None);

        Assert.Same(professionals, result);
        _repositoryMock.Verify(R => R.GetErasUsersByRoleAsync(ErasRole.Professional.Label), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRoleIsNull_ReturnsAllUsers()
    {
        var allUsers = new List<ErasUserDTO>
        {
            new()
            {
                Sub = "sub-1",
                Email = "a@test.com",
                FirstName = "A",
                LastName = "One",
                Role = ErasRole.Professional.Label,
                Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            },
            new()
            {
                Sub = "sub-2",
                Email = "b@test.com",
                FirstName = "B",
                LastName = "Two",
                Role = ErasRole.Officer.Label,
                Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            },
        };

        _repositoryMock
            .Setup(R => R.GetErasUsersByRoleAsync(null))
            .ReturnsAsync(allUsers);

        var result = await _handler.Handle(new GetErasUsersByRoleQuery(), CancellationToken.None);

        Assert.Same(allUsers, result);
        _repositoryMock.Verify(R => R.GetErasUsersByRoleAsync(null), Times.Once);
    }
}
