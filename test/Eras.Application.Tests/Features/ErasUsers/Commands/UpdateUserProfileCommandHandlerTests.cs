using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Commands;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Mock<IErasUsersRepository> _repositoryMock = new();
    private readonly Mock<IValidator<ErasUser>> _validatorMock = new();
    private readonly UpdateUserProfileCommandHandler _handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        _validatorMock
            .Setup(V => V.ValidateAsync(It.IsAny<ErasUser>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _handler = new UpdateUserProfileCommandHandler(_repositoryMock.Object, _validatorMock.Object);
    }

    private static ErasUserDTO CreateExistingProfile() => new()
    {
        Id = 1,
        Sub = "sub-123",
        Email = "user@test.com",
        FirstName = "First",
        LastName = "Last",
        Role = ErasRole.Officer.Label,
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    public async Task Handle_UpdatesOnlyEditableFields_AndLeavesIdentityFieldsUnchanged()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(CreateExistingProfile());
        _repositoryMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>())).ReturnsAsync((ErasUser Entity) => Entity);

        var command = new UpdateUserProfileCommand("sub-123", "user@test.com", "E-1", "IT", "555-0100", "Engineer", "Bio text");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("E-1", result.EmployeeId);
        Assert.Equal("IT", result.Department);
        Assert.Equal("555-0100", result.Phone);
        Assert.Equal("Engineer", result.Position);
        Assert.Equal("Bio text", result.About);
        Assert.Equal("First", result.FirstName);
        Assert.Equal("Last", result.LastName);
        Assert.Equal("user@test.com", result.Email);
        Assert.Equal(ErasRole.Officer.Label, result.Role);
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FallsBackToEmail_WhenSubLookupMisses()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(CreateExistingProfile());
        _repositoryMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>())).ReturnsAsync((ErasUser Entity) => Entity);

        var command = new UpdateUserProfileCommand("sub-123", "user@test.com", "E-1", "IT", "555-0100", "Engineer", "Bio text");
        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("E-1", result.EmployeeId);
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);

        var command = new UpdateUserProfileCommand("sub-123", "user@test.com", "E-1", "IT", "555-0100", "Engineer", "Bio text");

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Never);
    }
}
