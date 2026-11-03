using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.CommandHandlers;
using Eras.Application.Features.ErasUsers.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using FluentValidation;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Commands;

public class UpdateMyProfessionalProfileCommandHandlerTests
{
    private readonly Mock<IErasUsersRepository> _usersMock = new();
    private readonly Mock<IProfessionalProfileRepository> _profilesMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UpdateMyProfessionalProfileCommandHandler _handler;

    public UpdateMyProfessionalProfileCommandHandlerTests()
    {
        _unitOfWorkMock
            .Setup(U => U.ExecuteInTransactionAsync(It.IsAny<Func<Task<ProfessionalProfileDTO>>>()))
            .Returns((Func<Task<ProfessionalProfileDTO>> Work) => Work());

        _handler = new UpdateMyProfessionalProfileCommandHandler(
            _usersMock.Object,
            _profilesMock.Object,
            new ProfessionalProfileValidator(),
            new ErasUserValidator(),
            _unitOfWorkMock.Object);
    }

    private static ErasUserDTO CreateProfessional() => new()
    {
        Id = 7,
        Sub = "sub-pro",
        Email = "pro@test.com",
        FirstName = "Pat",
        LastName = "Rivera",
        Role = ErasRole.Professional.Label,
        Position = "Dean",
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    private static ProfessionalProfile CreateSavedProfile() => new()
    {
        Id = 3,
        ErasUserId = 7,
        Faculty = "Engineering",
        OfficeHours = "Mon 9:00-11:00",
        SubjectsTaught = ["Algorithms"],
        Courses = [new ProfessionalCourse { Id = 1, Code = "CS101", Name = "Intro to Programming" }],
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-4")]
    public async Task HandleAsync_EmptyFaculty_ThrowsValidationAndKeepsValuesAsync()
    {
        // Arrange (GIVEN) an ERAS Professional with a saved professional profile
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional());
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync(CreateSavedProfile());
        var request = new UpdateProfessionalProfileRequest
        {
            Position = "Dean",
            Faculty = string.Empty,
            OfficeHours = "Tue 14:00-16:00",
            SubjectsTaught = [],
            Courses = [],
        };
        var command = new UpdateMyProfessionalProfileCommand("sub-pro", "pro@test.com", request);

        // Act (WHEN) they submit an update where faculty is empty
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert (THEN) a validation error names faculty, and nothing is saved (AND)
        ValidationException error = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Contains(error.Errors, Failure => Failure.PropertyName == "Faculty");
        _profilesMock.Verify(R => R.SaveAsync(It.IsAny<ProfessionalProfile>()), Times.Never);
        _usersMock.Verify(R => R.UpdateAsync(It.IsAny<ErasUser>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-3")]
    public async Task HandleAsync_ValidProfile_ReplacesSavedValuesAsync()
    {
        // Arrange (GIVEN) an ERAS Professional with a saved professional profile
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional());
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync(CreateSavedProfile());
        ProfessionalProfile? saved = null;
        _profilesMock
            .Setup(R => R.SaveAsync(It.IsAny<ProfessionalProfile>()))
            .ReturnsAsync((ProfessionalProfile Profile) =>
            {
                saved = Profile;
                return Profile;
            });
        _usersMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>())).ReturnsAsync((ErasUser Entity) => Entity);
        var request = new UpdateProfessionalProfileRequest
        {
            Position = "Dean",
            Faculty = "Science",
            OfficeHours = "Tue 14:00-16:00",
            SubjectsTaught = ["Statistics"],
            Courses = [new ProfessionalCourseDTO("MATH200", "Statistics II")],
        };
        var command = new UpdateMyProfessionalProfileCommand("sub-pro", "pro@test.com", request);

        // Act (WHEN) they submit a complete profile
        ProfessionalProfileDTO result = await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN) the API returns the saved profile with the new values (AND) the previous courses are replaced
        Assert.Equal("Science", result.Faculty);
        Assert.Equal("Tue 14:00-16:00", result.OfficeHours);
        Assert.Equal(new[] { "Statistics" }, result.SubjectsTaught);
        Assert.Equal(new[] { "MATH200" }, result.Courses.Select(Course => Course.Code));
        Assert.NotNull(saved);
        Assert.Equal(new[] { "Statistics" }, saved!.SubjectsTaught);
        Assert.Single(saved.Courses);
        Assert.Equal("MATH200", saved.Courses[0].Code);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-7")]
    public async Task HandleAsync_Position_UpdatesUserPositionAsync()
    {
        // Arrange (GIVEN) an ERAS Professional whose position is empty
        ErasUserDTO professional = CreateProfessional();
        professional.Position = null;
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(professional);
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync((ProfessionalProfile?)null);
        _profilesMock.Setup(R => R.SaveAsync(It.IsAny<ProfessionalProfile>())).ReturnsAsync((ProfessionalProfile Profile) => Profile);
        ErasUser? updatedUser = null;
        _usersMock
            .Setup(R => R.UpdateAsync(It.IsAny<ErasUser>()))
            .ReturnsAsync((ErasUser Entity) =>
            {
                updatedUser = Entity;
                return Entity;
            });
        var request = new UpdateProfessionalProfileRequest
        {
            Position = "Dean",
            Faculty = "Science",
            OfficeHours = "Tue 14:00-16:00",
            SubjectsTaught = [],
            Courses = [],
        };
        var command = new UpdateMyProfessionalProfileCommand("sub-pro", "pro@test.com", request);

        // Act (WHEN) they save a professional profile with position "Dean"
        await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN) the position of the user is "Dean"
        Assert.NotNull(updatedUser);
        Assert.Equal("Dean", updatedUser!.Position);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-21")]
    public async Task HandleAsync_EmptyPositionAndOfficeHours_SavesAsync()
    {
        // Arrange (GIVEN) an ERAS Professional with a saved professional profile
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional());
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync(CreateSavedProfile());
        ProfessionalProfile? saved = null;
        _profilesMock
            .Setup(R => R.SaveAsync(It.IsAny<ProfessionalProfile>()))
            .ReturnsAsync((ProfessionalProfile Profile) =>
            {
                saved = Profile;
                return Profile;
            });
        ErasUser? updatedUser = null;
        _usersMock
            .Setup(R => R.UpdateAsync(It.IsAny<ErasUser>()))
            .ReturnsAsync((ErasUser Entity) =>
            {
                updatedUser = Entity;
                return Entity;
            });
        var request = new UpdateProfessionalProfileRequest
        {
            Position = string.Empty,
            Faculty = "Science",
            OfficeHours = string.Empty,
            SubjectsTaught = [],
            Courses = [],
        };
        var command = new UpdateMyProfessionalProfileCommand("sub-pro", "pro@test.com", request);

        // Act (WHEN) they submit a profile with faculty "Science" and empty position and office hours
        ProfessionalProfileDTO result = await _handler.Handle(command, CancellationToken.None);

        // Assert (THEN) the save succeeds (AND) the saved position and office hours are empty
        Assert.Equal("Science", result.Faculty);
        Assert.NotNull(saved);
        Assert.Equal(string.Empty, saved!.OfficeHours);
        Assert.NotNull(updatedUser);
        Assert.Equal(string.Empty, updatedUser!.Position);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-23")]
    public async Task HandleAsync_CourseWithEmptyName_ThrowsValidationAndKeepsValuesAsync()
    {
        // Arrange (GIVEN) an ERAS Professional with a saved professional profile
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional());
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync(CreateSavedProfile());
        _profilesMock.Setup(R => R.SaveAsync(It.IsAny<ProfessionalProfile>())).ReturnsAsync((ProfessionalProfile Profile) => Profile);
        _usersMock.Setup(R => R.UpdateAsync(It.IsAny<ErasUser>())).ReturnsAsync((ErasUser Entity) => Entity);
        var request = new UpdateProfessionalProfileRequest
        {
            Position = "Dean",
            Faculty = "Science",
            OfficeHours = "Tue 14:00-16:00",
            SubjectsTaught = [],
            Courses = [new ProfessionalCourseDTO("MATH200", string.Empty)],
        };
        var command = new UpdateMyProfessionalProfileCommand("sub-pro", "pro@test.com", request);

        // Act (WHEN) they submit a course whose name is empty
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert (THEN) a validation error names the courses field, and nothing is saved (AND)
        ValidationException error = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Contains(error.Errors, Failure => Failure.PropertyName.StartsWith("Courses", StringComparison.Ordinal));
        _profilesMock.Verify(R => R.SaveAsync(It.IsAny<ProfessionalProfile>()), Times.Never);
    }
}
