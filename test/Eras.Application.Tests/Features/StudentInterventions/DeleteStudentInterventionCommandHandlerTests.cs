using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.Features.StudentInterventions;
using Eras.Application.Features.StudentInterventions.Handlers;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.StudentInterventions;

public class DeleteStudentInterventionCommandHandlerTests
{

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentInterventionEntryRepository> _entries = new();
    private readonly Mock<IStudentAccessService> _access = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    public DeleteStudentInterventionCommandHandlerTests()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1, Name = "Ana Torres" });
    }

    private void SignInAs(string Sub, bool IsAdministrator = false)
    {
        _currentUser.SetupGet(User => User.Sub).Returns(Sub);
        _access.SetupGet(Service => Service.IsAdministrator).Returns(IsAdministrator);
    }

    private void StoreEntryCreatedBy(string Sub)
    {
        _entries.Setup(Repository => Repository.GetByIdAsync(5)).ReturnsAsync(new StudentInterventionEntry
        {
            Id = 5,
            StudentId = 1,
            Kind = StudentInterventionKind.SupportReceived,
            Type = StudentInterventionType.Tutoring,
            Description = "Weekly tutoring",
            EntryDate = new DateOnly(2026, 10, 21),
            CreatedByName = "Ana Pérez",
            Audit = new AuditInfo { CreatedBy = Sub, CreatedAt = DateTime.UtcNow },
        });
    }

    private DeleteStudentInterventionCommandHandler CreateHandler() =>
        new(_students.Object, _entries.Object, _access.Object, _currentUser.Object);

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-12")]
    public async Task HandleAsync_ProfessionalDeletesOwnEntry_SoftDeletesWithActorAsync()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Ana Pérez"
        SignInAs("ana-sub");
        StoreEntryCreatedBy("ana-sub");
        DeleteStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) "Ana Pérez" deletes it
        await handler.Handle(new DeleteStudentInterventionCommand(1, 5), CancellationToken.None);

        // Assert (THEN) the entry is soft deleted recording "Ana Pérez" as the user who deleted it
        _entries.Verify(Repository => Repository.SoftDeleteAsync(5, "ana-sub"), Times.Once);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-14")]
    public async Task HandleAsync_EntryOfOtherUser_Throws403AndKeepsEntryAsync()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Luis Gómez"
        SignInAs("ana-sub");
        StoreEntryCreatedBy("luis-sub");
        DeleteStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) "Ana Pérez" sends a delete of that entry
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new DeleteStudentInterventionCommand(1, 5), CancellationToken.None));

        // Assert (THEN) 403 and the entry is not deleted
        Assert.Equal(403, error.StatusCode);
        _entries.Verify(Repository => Repository.SoftDeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-28")]
    public async Task HandleAsync_AdministratorDeletesEntryOfProfessional_SoftDeletesWithAdministratorAsync()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Ana Pérez" and an ERAS Administrator
        SignInAs("admin-sub", IsAdministrator: true);
        StoreEntryCreatedBy("ana-sub");
        DeleteStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) the Administrator deletes it
        await handler.Handle(new DeleteStudentInterventionCommand(1, 5), CancellationToken.None);

        // Assert (THEN) the entry is soft deleted with the Administrator as the user who deleted it
        _entries.Verify(Repository => Repository.SoftDeleteAsync(5, "admin-sub"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UnknownEntry_Throws404Async()
    {
        SignInAs("ana-sub");
        _entries.Setup(Repository => Repository.GetByIdAsync(99)).ReturnsAsync((StudentInterventionEntry?)null);
        DeleteStudentInterventionCommandHandler handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new DeleteStudentInterventionCommand(1, 99), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_UnassignedStudent_Throws403AndDoesNotDeleteAsync()
    {
        SignInAs("ana-sub");
        StoreEntryCreatedBy("ana-sub");
        _access
            .Setup(Service => Service.EnsureCanAccessStudentAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BussinessException("You do not have access to this student.", 403));
        DeleteStudentInterventionCommandHandler handler = CreateHandler();

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new DeleteStudentInterventionCommand(1, 5), CancellationToken.None));

        Assert.Equal(403, error.StatusCode);
        _entries.Verify(Repository => Repository.SoftDeleteAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
