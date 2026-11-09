using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.UsersManagement;
using Eras.Application.Features.ErasUsers;
using Eras.Application.Features.ErasUsers.Handlers.QueryHandlers;
using Eras.Domain.Common;
using Eras.Domain.Entities.UserManagement;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.ErasUsers.Queries;

public class GetMyProfessionalProfileQueryHandlerTests
{
    private readonly Mock<IErasUsersRepository> _usersMock = new();
    private readonly Mock<IProfessionalProfileRepository> _profilesMock = new();
    private readonly GetMyProfessionalProfileQueryHandler _handler;

    public GetMyProfessionalProfileQueryHandlerTests()
    {
        _handler = new GetMyProfessionalProfileQueryHandler(_usersMock.Object, _profilesMock.Object);
    }

    private static ErasUserDTO CreateProfessional(string? position) => new()
    {
        Id = 7,
        Sub = "sub-pro",
        Email = "pro@test.com",
        FirstName = "Pat",
        LastName = "Rivera",
        Role = ErasRole.Professional.Label,
        Position = position,
        Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
    };

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-1")]
    public async Task HandleAsync_SavedProfile_ReturnsAllFieldsAsync()
    {
        // Arrange (GIVEN) an ERAS Professional who has saved a profile
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional("Dean"));
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync(new ProfessionalProfile
        {
            Id = 3,
            ErasUserId = 7,
            Faculty = "Engineering",
            OfficeHours = "Mon 9:00-11:00",
            SubjectsTaught = ["Algorithms", "Databases"],
            Courses = [new ProfessionalCourse { Id = 1, Code = "CS101", Name = "Intro to Programming" }],
        });

        // Act (WHEN) they request their professional profile
        ProfessionalProfileDTO result = await _handler.Handle(
            new GetMyProfessionalProfileQuery("sub-pro", "pro@test.com"),
            CancellationToken.None);

        // Assert (THEN) the saved values are returned, with the position of the user (AND) nothing is changed
        Assert.Equal("Dean", result.Position);
        Assert.Equal("Engineering", result.Faculty);
        Assert.Equal("Mon 9:00-11:00", result.OfficeHours);
        Assert.Equal(new[] { "Algorithms", "Databases" }, result.SubjectsTaught);
        Assert.Equal(new[] { new ProfessionalCourseDTO("CS101", "Intro to Programming") }, result.Courses);
        _profilesMock.Verify(R => R.SaveAsync(It.IsAny<ProfessionalProfile>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "603-persist-professional-profile")]
    [Trait("AC", "603-persist-professional-profile/AC-2")]
    public async Task HandleAsync_NeverSaved_ReturnsEmptyValuesAsync()
    {
        // Arrange (GIVEN) an ERAS Professional who has never saved a professional profile and has no position
        _usersMock.Setup(R => R.GetErasUserBySubAsync("sub-pro")).ReturnsAsync(CreateProfessional(null));
        _profilesMock.Setup(R => R.GetByErasUserIdAsync(7)).ReturnsAsync((ProfessionalProfile?)null);

        // Act (WHEN) they request their professional profile
        ProfessionalProfileDTO result = await _handler.Handle(
            new GetMyProfessionalProfileQuery("sub-pro", "pro@test.com"),
            CancellationToken.None);

        // Assert (THEN) the text values are empty and the lists are empty
        Assert.Equal(string.Empty, result.Position);
        Assert.Equal(string.Empty, result.Faculty);
        Assert.Equal(string.Empty, result.OfficeHours);
        Assert.Empty(result.SubjectsTaught);
        Assert.Empty(result.Courses);
    }
}
