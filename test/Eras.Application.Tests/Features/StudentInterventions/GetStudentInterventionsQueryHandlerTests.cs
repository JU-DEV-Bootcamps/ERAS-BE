using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Features.StudentInterventions;
using Eras.Application.Features.StudentInterventions.Handlers;
using Eras.Application.Mappers.StudentInterventions;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.StudentInterventions;

public class GetStudentInterventionsQueryHandlerTests
{

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentInterventionEntryRepository> _entries = new();
    private readonly Mock<IStudentAccessService> _access = new();

    public GetStudentInterventionsQueryHandlerTests()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1, Name = "Ana Torres" });
    }

    private GetStudentInterventionsQueryHandler CreateHandler() =>
        new(_students.Object, _entries.Object, _access.Object, new StudentInterventionToDtoMapper());

    private static StudentInterventionEntry NewEntry(int Id) => new()
    {
        Id = Id,
        StudentId = 1,
        Kind = StudentInterventionKind.RiskIdentified,
        Type = StudentInterventionType.Attendance,
        Description = "Missed three classes",
        EntryDate = new DateOnly(2026, 10, 15),
        CreatedByName = "Ana Pérez",
        Audit = new AuditInfo { CreatedBy = "ana-sub", CreatedAt = new DateTime(2026, 10, 15, 9, 0, 0, DateTimeKind.Utc) },
    };

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-17")]
    public async Task HandleAsync_OfficerRequestsEntries_ReturnsThemAsync()
    {
        // Arrange (GIVEN) the student "Ana Torres" with one risk entry and an ERAS Student Services Officer
        _entries
            .Setup(Repository => Repository.GetByStudentIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([NewEntry(7)]);
        GetStudentInterventionsQueryHandler handler = CreateHandler();

        // Act (WHEN) the Officer requests the entries of "Ana Torres"
        IReadOnlyList<StudentInterventionDto> result = await handler.Handle(
            new GetStudentInterventionsQuery(1), CancellationToken.None);

        // Assert (THEN) that entry is returned, unchanged
        StudentInterventionDto entry = Assert.Single(result);
        Assert.Equal(7, entry.Id);
        Assert.Equal(StudentInterventionType.Attendance, entry.Type);
        Assert.Equal("Missed three classes", entry.Description);
        Assert.Equal("Ana Pérez", entry.CreatedByName);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-32")]
    public async Task HandleAsync_UnassignedStudent_Throws403Async()
    {
        // Arrange (GIVEN) a Professional assigned to no assessment of the student, who has one entry
        _access
            .Setup(Service => Service.EnsureCanAccessStudentAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BussinessException("You do not have access to this student.", 403));
        GetStudentInterventionsQueryHandler handler = CreateHandler();

        // Act (WHEN) they request the entries of that student
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new GetStudentInterventionsQuery(1), CancellationToken.None));

        // Assert (THEN) 403 and no entry is read
        Assert.Equal(403, error.StatusCode);
        _entries.Verify(
            Repository => Repository.GetByStudentIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudent_Throws404Async()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(999)).ReturnsAsync((Student?)null);
        GetStudentInterventionsQueryHandler handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetStudentInterventionsQuery(999), CancellationToken.None));
    }
}
