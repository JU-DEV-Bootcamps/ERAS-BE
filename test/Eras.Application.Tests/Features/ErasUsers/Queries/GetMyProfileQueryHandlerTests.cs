using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
using Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;
using Eras.Error.Bussiness;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Queries;

public class GetMyProfileQueryHandlerTests
{
    private readonly Mock<IErasUsersRepository> _repositoryMock = new();
    private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock = new();
    private readonly GetMyProfileQueryHandler _handler;

    public GetMyProfileQueryHandlerTests()
    {
        _handler = new GetMyProfileQueryHandler(_repositoryMock.Object, _assessmentRepositoryMock.Object);
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
        Department = "IT",
        Phone = "+1 555 123 4567",
        Position = "Coordinator",
        About = "Bio",
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    public async Task Handle_ResolvesBySub_WhenSubIsProvided()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        Assert.Equal("sub-123", result.Sub);
        Assert.Equal("E-1", result.EmployeeId);
        _repositoryMock.Verify(R => R.GetErasUserByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_FallsBackToEmail_WhenSubLookupMisses()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        Assert.Equal(profile.Email, result.Email);
    }

    [Fact]
    public async Task Handle_ThrowsNotFound_WhenUserDoesNotExist()
    {
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((ErasUserDTO?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsTheEditableFieldsAndTheActiveWorkCounters()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(profile);
        _assessmentRepositoryMock.Setup(R => R.CountActiveAssessmentsForUserAsync("sub-123")).ReturnsAsync(4);
        _assessmentRepositoryMock.Setup(R => R.CountActiveInterventionsForUserAsync("sub-123")).ReturnsAsync(7);

        var result = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        Assert.Equal("First", result.FirstName);
        Assert.Equal("Last", result.LastName);
        Assert.Equal(ErasRole.Officer.Label, result.Role);
        Assert.Equal("IT", result.Department);
        Assert.Equal("+1 555 123 4567", result.Phone);
        Assert.Equal("Coordinator", result.Position);
        Assert.Equal("Bio", result.About);
        Assert.Equal(4, result.ActiveAssessmentsCount);
        Assert.Equal(7, result.ActiveInterventionsCount);
    }

    [Fact]
    public async Task Handle_CountsWithTheStoredSub_WhenTheRequestOnlyHadTheEmail()
    {
        var profile = CreateProfile();
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(profile);
        _assessmentRepositoryMock.Setup(R => R.CountActiveAssessmentsForUserAsync("sub-123")).ReturnsAsync(2);

        var result = await _handler.Handle(new GetMyProfileQuery(null, "user@test.com"), CancellationToken.None);

        Assert.Equal(2, result.ActiveAssessmentsCount);
    }

    [Fact]
    public async Task Handle_UserWithoutSub_HasNoActiveWorkAndDoesNotQueryIt()
    {
        var profile = CreateProfile();
        profile.Sub = null;
        _repositoryMock.Setup(R => R.GetErasUserByEmailAsync("user@test.com")).ReturnsAsync(profile);

        var result = await _handler.Handle(new GetMyProfileQuery(null, "user@test.com"), CancellationToken.None);

        Assert.Equal(0, result.ActiveAssessmentsCount);
        Assert.Equal(0, result.ActiveInterventionsCount);
        _assessmentRepositoryMock.Verify(R => R.CountActiveAssessmentsForUserAsync(It.IsAny<string>()), Times.Never);
        _assessmentRepositoryMock.Verify(R => R.CountActiveInterventionsForUserAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-18")]
    public async Task HandleAsync_AfterProfessionalUpdate_ReportsSameActiveInterventionsAsync()
    {
        // Arrange (GIVEN) an ERAS Professional who has 2 active interventions they created
        ErasUserDTO professional = CreateProfile();
        professional.Role = ErasRole.Professional.Label;
        professional.Position = "Dean";
        _repositoryMock.Setup(R => R.GetErasUserBySubAsync("sub-123")).ReturnsAsync(professional);
        _repositoryMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>())).ReturnsAsync((ErasUser Entity) => Entity);
        _assessmentRepositoryMock.Setup(R => R.CountActiveInterventionsForUserAsync("sub-123")).ReturnsAsync(2);

        var profilesMock = new Mock<IProfessionalProfileRepository>();
        profilesMock.Setup(R => R.GetByErasUserIdAsync(It.IsAny<int>())).ReturnsAsync((ProfessionalProfile?)null);
        profilesMock.Setup(R => R.SaveAsync(It.IsAny<ProfessionalProfile>())).ReturnsAsync((ProfessionalProfile Profile) => Profile);
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<ProfessionalProfileDTO>>>()))
            .Returns((Func<Task<ProfessionalProfileDTO>> Work) => Work());
        var updateHandler = new UpdateMyProfessionalProfileCommandHandler(
            _repositoryMock.Object,
            profilesMock.Object,
            new ProfessionalProfileValidator(),
            new ErasUserValidator(),
            unitOfWorkMock.Object);

        var before = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        // Act (WHEN) they submit a valid professional profile update
        var update = new UpdateMyProfessionalProfileCommand(
            "sub-123",
            "user@test.com",
            new UpdateProfessionalProfileRequest
            {
                Position = "Dean",
                Faculty = "Science",
                OfficeHours = "Tue 14:00-16:00",
                SubjectsTaught = [],
                Courses = [],
            });
        await updateHandler.Handle(update, CancellationToken.None);
        var after = await _handler.Handle(new GetMyProfileQuery("sub-123", "user@test.com"), CancellationToken.None);

        // Assert (THEN) the general profile still reports 2 active interventions, before and after the update
        Assert.Equal(2, before.ActiveInterventionsCount);
        Assert.Equal(2, after.ActiveInterventionsCount);
    }
}
