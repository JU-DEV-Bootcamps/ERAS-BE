using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Commands;

public class SyncErasUserCommandHandlerTests
{
    private readonly Mock<IErasUsersRepository> _repositoryMock = new();
    private readonly Mock<IValidator<ErasUser>> _validatorMock = new();
    private readonly SyncErasUserCommandHandler _handler;

    public SyncErasUserCommandHandlerTests()
    {
        _validatorMock
            .Setup(V => V.ValidateAsync(It.IsAny<ErasUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new SyncErasUserCommandHandler(_repositoryMock.Object, _validatorMock.Object);
    }

    private static SyncErasUserCommand CreateCommand(string? sub = "sub-123") =>
        new(sub, "user@test.com", "First", "Last", ErasRole.Professional.Label);

    [Fact]
    public async Task Handle_UserDoesNotExist_CreatesNewUser()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync(It.IsAny<string>()))
            .ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.AddAsync(It.IsAny<ErasUser>()))
            .ReturnsAsync((ErasUser Entity) => Entity);

        var command = CreateCommand();
        ErasUserDTO result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(command.Email, result.Email);
        Assert.Equal(command.Role, result.Role);
        Assert.True(result.IsSynced);
        _repositoryMock.Verify(R => R.AddAsync(It.IsAny<ErasUser>()), Times.Once);
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserExistsAndAlreadySynced_RefreshesRoleFromKeycloak()
    {
        // Keycloak is the source of truth for role assignment: a user starts as Guest
        // and gets a real role assigned there later, so every login must refresh the
        // role, not just the first one.
        var existing = new ErasUserDTO
        {
            Id = 1,
            Sub = "sub-123",
            Email = "user@test.com",
            FirstName = "First",
            LastName = "Last",
            Role = ErasRole.Guest.Label,
            IsSynced = true,
            Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        };

        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(existing);
        _repositoryMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>()))
            .ReturnsAsync((ErasUser Entity) => Entity);

        var command = CreateCommand();
        ErasUserDTO result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(ErasRole.Professional.Label, result.Role);
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Once);
        _repositoryMock.Verify(R => R.AddAsync(It.IsAny<ErasUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UserExistsButNotSynced_UpdatesUser()
    {
        var existing = new ErasUserDTO
        {
            Id = 1,
            Sub = null,
            Email = "user@test.com",
            FirstName = "Old",
            LastName = "Name",
            Role = ErasRole.Guest.Label,
            IsSynced = false,
            Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
        };

        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(existing);
        _repositoryMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>()))
            .ReturnsAsync((ErasUser Entity) => Entity);

        var command = CreateCommand();
        ErasUserDTO result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSynced);
        Assert.Equal("sub-123", result.Sub);
        Assert.Equal(command.Role, result.Role);
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Once);
        _repositoryMock.Verify(R => R.AddAsync(It.IsAny<ErasUser>()), Times.Never);
    }
}
