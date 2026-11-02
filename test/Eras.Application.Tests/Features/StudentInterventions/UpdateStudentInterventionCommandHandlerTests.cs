using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Features.StudentInterventions;
using Eras.Application.Features.StudentInterventions.Handlers;
using Eras.Application.Mappers.StudentInterventions;
using Eras.Application.Validation;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.StudentInterventions;

public class UpdateStudentInterventionCommandHandlerTests
{
    private static readonly DateTime OriginalCreatedAt = new(2026, 10, 1, 9, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentInterventionEntryRepository> _entries = new();
    private readonly Mock<IStudentAccessService> _access = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private StudentInterventionEntry? _updated;

    public UpdateStudentInterventionCommandHandlerTests()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1, Name = "Ana Torres" });
        _entries
            .Setup(Repository => Repository.UpdateAsync(It.IsAny<StudentInterventionEntry>()))
            .ReturnsAsync((StudentInterventionEntry Entry) =>
            {
                _updated = Entry;
                return Entry;
            });
    }

    private void SignInAs(string Sub, string Name, bool IsAdministrator = false)
    {
        _currentUser.SetupGet(User => User.Sub).Returns(Sub);
        _currentUser.SetupGet(User => User.Name).Returns(Name);
        _access.SetupGet(Service => Service.IsAdministrator).Returns(IsAdministrator);
    }

    private void StoreEntryCreatedBy(string Sub, string Name)
    {
        _entries.Setup(Repository => Repository.GetByIdAsync(5)).ReturnsAsync(new StudentInterventionEntry
        {
            Id = 5,
            StudentId = 1,
            Kind = StudentInterventionKind.RiskIdentified,
            Type = StudentInterventionType.Academic,
            Description = "Failed two midterms",
            EntryDate = new DateOnly(2026, 10, 20),
            CreatedByName = Name,
            Audit = new AuditInfo { CreatedBy = Sub, CreatedAt = OriginalCreatedAt },
        });
    }

    private UpdateStudentInterventionCommandHandler CreateHandler() => new(
        _students.Object,
        _entries.Object,
        _access.Object,
        _currentUser.Object,
        new SaveStudentInterventionDtoValidator(),
        new StudentInterventionToDtoMapper());

    private static SaveStudentInterventionDto NewDescription(string Description) => new()
    {
        Kind = StudentInterventionKind.RiskIdentified,
        Type = StudentInterventionType.Academic,
        Description = Description,
        EntryDate = new DateOnly(2026, 10, 20),
    };

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-10")]
    public async Task HandleAsync_ProfessionalEditsOwnEntry_KeepsCreatorAndCreationTimeAsync()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Ana Pérez" with description "Failed two midterms"
        SignInAs("ana-sub", "Ana Pérez");
        StoreEntryCreatedBy("ana-sub", "Ana Pérez");
        UpdateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) "Ana Pérez" changes the description to "Failed three midterms" and saves
        StudentInterventionDto result = await handler.Handle(
            new UpdateStudentInterventionCommand(1, 5, NewDescription("Failed three midterms")), CancellationToken.None);

        // Assert (THEN) the entry shows the new description and keeps its creator and creation time
        Assert.Equal("Failed three midterms", result.Description);
        Assert.Equal("Failed three midterms", _updated!.Description);
        Assert.Equal("ana-sub", _updated.Audit.CreatedBy);
        Assert.Equal("Ana Pérez", _updated.CreatedByName);
        Assert.Equal(OriginalCreatedAt, _updated.Audit.CreatedAt);
        Assert.Equal("ana-sub", _updated.Audit.ModifiedBy);
        Assert.NotNull(_updated.Audit.ModifiedAt);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-11")]
    public async Task HandleAsync_EntryOfOtherUser_Throws403Async()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Luis Gómez"
        SignInAs("ana-sub", "Ana Pérez");
        StoreEntryCreatedBy("luis-sub", "Luis Gómez");
        UpdateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) "Ana Pérez" sends an edit of that entry
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => handler.Handle(
            new UpdateStudentInterventionCommand(1, 5, NewDescription("Changed")), CancellationToken.None));

        // Assert (THEN) 403 and the entry keeps its previous values
        Assert.Equal(403, error.StatusCode);
        _entries.Verify(Repository => Repository.UpdateAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-27")]
    public async Task HandleAsync_AdministratorEditsEntryOfProfessional_KeepsOriginalCreatorAsync()
    {
        // Arrange (GIVEN) an entry created by the ERAS Professional "Ana Pérez" and an ERAS Administrator
        SignInAs("admin-sub", "Admin User", IsAdministrator: true);
        StoreEntryCreatedBy("ana-sub", "Ana Pérez");
        UpdateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) the Administrator changes its description and saves
        StudentInterventionDto result = await handler.Handle(
            new UpdateStudentInterventionCommand(1, 5, NewDescription("Reviewed by admin")), CancellationToken.None);

        // Assert (THEN) 200 with the new description and "Ana Pérez" still the creator
        Assert.Equal("Reviewed by admin", result.Description);
        Assert.Equal("ana-sub", _updated!.Audit.CreatedBy);
        Assert.Equal("Ana Pérez", _updated.CreatedByName);
        Assert.Equal(OriginalCreatedAt, _updated.Audit.CreatedAt);
    }

    [Fact]
    public async Task HandleAsync_EntryOfAnotherStudent_Throws404Async()
    {
        SignInAs("ana-sub", "Ana Pérez");
        StoreEntryCreatedBy("ana-sub", "Ana Pérez");
        _students.Setup(Repository => Repository.GetByIdAsync(2)).ReturnsAsync(new Student { Id = 2, Name = "Luis Anaya" });
        UpdateStudentInterventionCommandHandler handler = CreateHandler();

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new UpdateStudentInterventionCommand(2, 5, NewDescription("Changed")), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_StoredKindWinsOverTheBodyKind_Async()
    {
        SignInAs("ana-sub", "Ana Pérez");
        StoreEntryCreatedBy("ana-sub", "Ana Pérez");
        UpdateStudentInterventionCommandHandler handler = CreateHandler();
        SaveStudentInterventionDto body = NewDescription("Changed");
        body.Kind = StudentInterventionKind.SupportReceived;

        StudentInterventionDto result = await handler.Handle(new UpdateStudentInterventionCommand(1, 5, body), CancellationToken.None);

        Assert.Equal(StudentInterventionKind.RiskIdentified, result.Kind);
    }

    [Fact]
    public async Task HandleAsync_InvalidDescription_Throws400AndDoesNotUpdateAsync()
    {
        SignInAs("ana-sub", "Ana Pérez");
        StoreEntryCreatedBy("ana-sub", "Ana Pérez");
        UpdateStudentInterventionCommandHandler handler = CreateHandler();

        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() => handler.Handle(
            new UpdateStudentInterventionCommand(1, 5, NewDescription("  ")), CancellationToken.None));

        Assert.Equal(400, error.StatusCode);
        _entries.Verify(Repository => Repository.UpdateAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }
}
