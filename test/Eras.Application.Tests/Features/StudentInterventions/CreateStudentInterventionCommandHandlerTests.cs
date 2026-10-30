using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Contracts.Services;
using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Features.StudentInterventions;
using Eras.Application.Features.StudentInterventions.Handlers;
using Eras.Application.Mappers.StudentInterventions;
using Eras.Application.Validation;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using Moq;

namespace Eras.Application.Tests.Features.StudentInterventions;

public class CreateStudentInterventionCommandHandlerTests
{

    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IStudentInterventionEntryRepository> _entries = new();
    private readonly Mock<IStudentAccessService> _access = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private StudentInterventionEntry? _saved;

    public CreateStudentInterventionCommandHandlerTests()
    {
        _students.Setup(Repository => Repository.GetByIdAsync(1)).ReturnsAsync(new Student { Id = 1, Name = "Ana Torres" });
        _currentUser.SetupGet(User => User.Sub).Returns("ana-sub");
        _currentUser.SetupGet(User => User.Name).Returns("Ana Pérez");
        _entries
            .Setup(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()))
            .ReturnsAsync((StudentInterventionEntry Entry) =>
            {
                Entry.Id = 10;
                _saved = Entry;
                return Entry;
            });
    }

    private CreateStudentInterventionCommandHandler CreateHandler() => new(
        _students.Object,
        _entries.Object,
        _access.Object,
        _currentUser.Object,
        new SaveStudentInterventionDtoValidator(),
        new StudentInterventionToDtoMapper());

    private static SaveStudentInterventionDto ValidRisk() => new()
    {
        Kind = StudentInterventionKind.RiskIdentified,
        Type = StudentInterventionType.Academic,
        Description = "Failed two midterms",
        EntryDate = new DateOnly(2026, 10, 20),
    };

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-1")]
    public async Task HandleAsync_ProfessionalAddsRisk_PersistsEntryForStudentAsync()
    {
        // Arrange (GIVEN) an ERAS Professional "Ana Pérez" with the student "Ana Torres" open
        CreateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) they add a risk "Academic", "Failed two midterms", 2026-10-20
        StudentInterventionDto result = await handler.Handle(
            new CreateStudentInterventionCommand(1, ValidRisk()), CancellationToken.None);

        // Assert (THEN) the entry is saved for "Ana Torres" and returned
        Assert.NotNull(_saved);
        Assert.Equal(1, _saved!.StudentId);
        Assert.Equal(StudentInterventionKind.RiskIdentified, _saved.Kind);
        Assert.Equal(StudentInterventionType.Academic, _saved.Type);
        Assert.Equal("Failed two midterms", _saved.Description);
        Assert.Equal(new DateOnly(2026, 10, 20), _saved.EntryDate);
        Assert.Equal(10, result.Id);
        Assert.Equal(1, result.StudentId);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-2")]
    public async Task HandleAsync_ProfessionalAddsSupport_ReturnsSupportEntryAsync()
    {
        // Arrange (GIVEN) an ERAS Professional and the student "Ana Torres"
        CreateStudentInterventionCommandHandler handler = CreateHandler();
        SaveStudentInterventionDto support = new()
        {
            Kind = StudentInterventionKind.SupportReceived,
            Type = StudentInterventionType.Tutoring,
            Description = "Weekly math tutoring",
            EntryDate = new DateOnly(2026, 10, 21),
        };

        // Act (WHEN) they add a support entry "Tutoring"
        StudentInterventionDto result = await handler.Handle(
            new CreateStudentInterventionCommand(1, support), CancellationToken.None);

        // Assert (THEN) the entry is a support entry of "Ana Torres"
        Assert.Equal(StudentInterventionKind.SupportReceived, result.Kind);
        Assert.Equal(StudentInterventionType.Tutoring, result.Type);
        Assert.Equal(1, result.StudentId);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-3")]
    public async Task HandleAsync_AnyRequest_RecordsAuthenticatedUserAndServerTimeAsync()
    {
        // Arrange (GIVEN) "Ana Pérez" is the authenticated user; the request body has no creator or time to trust
        CreateStudentInterventionCommandHandler handler = CreateHandler();
        DateTime before = DateTime.UtcNow;

        // Act (WHEN) the entry is saved
        StudentInterventionDto result = await handler.Handle(
            new CreateStudentInterventionCommand(1, ValidRisk()), CancellationToken.None);

        // Assert (THEN) the creator is the token user and the time is the server's
        Assert.Equal("ana-sub", _saved!.Audit.CreatedBy);
        Assert.Equal("Ana Pérez", _saved.CreatedByName);
        Assert.InRange(_saved.Audit.CreatedAt, before, DateTime.UtcNow);
        Assert.Equal("ana-sub", result.CreatedBy);
        Assert.Equal("Ana Pérez", result.CreatedByName);
    }

    public static TheoryData<SaveStudentInterventionDto, string> MissingRequiredFields()
    {
        SaveStudentInterventionDto noType = ValidRisk();
        noType.Type = null;
        SaveStudentInterventionDto blankDescription = ValidRisk();
        blankDescription.Description = "   ";
        SaveStudentInterventionDto emptyDescription = ValidRisk();
        emptyDescription.Description = "";
        SaveStudentInterventionDto noDate = ValidRisk();
        noDate.EntryDate = null;
        return new TheoryData<SaveStudentInterventionDto, string>
        {
            { noType, "Type" },
            { blankDescription, "Description" },
            { emptyDescription, "Description" },
            { noDate, "Date" },
        };
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFields))]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-4")]
    public async Task HandleAsync_MissingRequiredField_Throws400NamingFieldAndDoesNotPersistAsync(
        SaveStudentInterventionDto Entry,
        string Field)
    {
        // Arrange (GIVEN) an ERAS Professional and the student "Ana Torres"
        CreateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) they save an entry with no type, or an empty or blank description, or no date
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new CreateStudentInterventionCommand(1, Entry), CancellationToken.None));

        // Assert (THEN) 400 naming the missing field and nothing persisted
        Assert.Equal(400, error.StatusCode);
        Assert.Contains(Field, error.FriendlyMessage);
        _entries.Verify(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-6")]
    public async Task HandleAsync_DescriptionOver1000Chars_Throws400Async()
    {
        // Arrange (GIVEN) an ERAS Professional and the student "Ana Torres"
        CreateStudentInterventionCommandHandler handler = CreateHandler();
        SaveStudentInterventionDto entry = ValidRisk();
        entry.Description = new string('a', 1001);

        // Act (WHEN) they save an entry whose description has 1001 characters
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new CreateStudentInterventionCommand(1, entry), CancellationToken.None));

        // Assert (THEN) 400 saying the description is too long and nothing persisted
        Assert.Equal(400, error.StatusCode);
        Assert.Contains("too long", error.FriendlyMessage);
        _entries.Verify(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-7")]
    public async Task HandleAsync_TypeOfOtherKind_Throws400NamingTypeAsync()
    {
        // Arrange (GIVEN) an ERAS Professional and the student "Ana Torres"
        CreateStudentInterventionCommandHandler handler = CreateHandler();
        SaveStudentInterventionDto entry = ValidRisk();
        entry.Type = StudentInterventionType.Tutoring;

        // Act (WHEN) they save a risk entry with the support type "Tutoring"
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new CreateStudentInterventionCommand(1, entry), CancellationToken.None));

        // Assert (THEN) 400 naming the Type field and nothing persisted
        Assert.Equal(400, error.StatusCode);
        Assert.Contains("Type", error.FriendlyMessage);
        _entries.Verify(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-8")]
    public async Task HandleAsync_UnknownStudent_Throws404Async()
    {
        // Arrange (GIVEN) no student with id 999999
        _students.Setup(Repository => Repository.GetByIdAsync(999999)).ReturnsAsync((Student?)null);
        CreateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) an ERAS Professional adds an entry to the student 999999
        NotFoundException error = await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new CreateStudentInterventionCommand(999999, ValidRisk()), CancellationToken.None));

        // Assert (THEN) 404 and nothing persisted
        Assert.Equal(404, error.StatusCode);
        _entries.Verify(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-21")]
    public async Task HandleAsync_AdministratorAddsEntry_RecordsAdministratorAsCreatorAsync()
    {
        // Arrange (GIVEN) an ERAS Administrator and the student "Ana Torres"
        _currentUser.SetupGet(User => User.Sub).Returns("admin-sub");
        _currentUser.SetupGet(User => User.Name).Returns("Admin User");
        CreateStudentInterventionCommandHandler handler = CreateHandler();
        SaveStudentInterventionDto entry = ValidRisk();
        entry.Type = StudentInterventionType.Financial;
        entry.Description = "Tuition payment overdue";
        entry.EntryDate = new DateOnly(2026, 10, 22);

        // Act (WHEN) they add a "Financial" risk
        StudentInterventionDto result = await handler.Handle(
            new CreateStudentInterventionCommand(1, entry), CancellationToken.None);

        // Assert (THEN) the entry shows the Administrator as its creator
        Assert.Equal("admin-sub", result.CreatedBy);
        Assert.Equal("Admin User", result.CreatedByName);
        Assert.Equal(1, _saved!.StudentId);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-33")]
    public async Task HandleAsync_UnassignedStudent_Throws403AndDoesNotPersistAsync()
    {
        // Arrange (GIVEN) a Professional assigned to no assessment of the student
        _access
            .Setup(Service => Service.EnsureCanAccessStudentAsync(1, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BussinessException("You do not have access to this student.", 403));
        CreateStudentInterventionCommandHandler handler = CreateHandler();

        // Act (WHEN) they add a risk entry
        BussinessException error = await Assert.ThrowsAsync<BussinessException>(() =>
            handler.Handle(new CreateStudentInterventionCommand(1, ValidRisk()), CancellationToken.None));

        // Assert (THEN) 403 and no entry persisted
        Assert.Equal(403, error.StatusCode);
        _entries.Verify(Repository => Repository.AddAsync(It.IsAny<StudentInterventionEntry>()), Times.Never);
    }
}
