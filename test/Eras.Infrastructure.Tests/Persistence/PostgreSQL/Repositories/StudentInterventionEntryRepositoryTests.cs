using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentInterventionEntryRepositoryTests : RepositoryTestBase
{

    private static StudentInterventionEntry NewEntry(
        int StudentId,
        StudentInterventionKind Kind,
        StudentInterventionType Type,
        DateOnly EntryDate,
        DateTime? CreatedAt = null) => new()
        {
            StudentId = StudentId,
            Kind = Kind,
            Type = Type,
            Description = "Some description",
            EntryDate = EntryDate,
            CreatedByName = "Ana Pérez",
            Audit = new AuditInfo { CreatedBy = "sub-ana", CreatedAt = CreatedAt ?? DateTime.UtcNow },
        };

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-15")]
    public async Task GetByStudentIdAsync_ManyEntries_ReturnsNewestFirstAsync()
    {
        // Arrange (GIVEN) a student with a risk dated 2026-10-01, a support dated 2026-10-20 and a risk dated 2026-10-10
        using AppDbContext context = CreateContext();
        context.StudentInterventionEntries.AddRange(
            NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Academic, new DateOnly(2026, 10, 1)),
            NewEntry(1, StudentInterventionKind.SupportReceived, StudentInterventionType.Tutoring, new DateOnly(2026, 10, 20)),
            NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Personal, new DateOnly(2026, 10, 10)));
        await context.SaveChangesAsync();
        StudentInterventionEntryRepository repository = new(context);

        // Act (WHEN) the entries of the student are requested
        IReadOnlyList<StudentInterventionEntry> result = await repository.GetByStudentIdAsync(1);

        // Assert (THEN) they come newest first
        Assert.Equal(
            [new DateOnly(2026, 10, 20), new DateOnly(2026, 10, 10), new DateOnly(2026, 10, 1)],
            result.Select(Entry => Entry.EntryDate).ToArray());
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-16")]
    public async Task GetByStudentIdAsync_OtherStudentEntries_AreNotReturnedAsync()
    {
        // Arrange (GIVEN) "Ana Torres" (1) with two entries and "Luis Anaya" (2) with none
        using AppDbContext context = CreateContext();
        context.StudentInterventionEntries.AddRange(
            NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Academic, new DateOnly(2026, 10, 1)),
            NewEntry(1, StudentInterventionKind.SupportReceived, StudentInterventionType.Advising, new DateOnly(2026, 10, 2)));
        await context.SaveChangesAsync();
        StudentInterventionEntryRepository repository = new(context);

        // Act (WHEN) the entries of "Luis Anaya" are requested
        IReadOnlyList<StudentInterventionEntry> luis = await repository.GetByStudentIdAsync(2);
        IReadOnlyList<StudentInterventionEntry> ana = await repository.GetByStudentIdAsync(1);

        // Assert (THEN) the list is empty and the entries of "Ana Torres" are still two
        Assert.Empty(luis);
        Assert.Equal(2, ana.Count);
    }

    [Fact]
    [Trait("Change", "643-add-student-interventions")]
    [Trait("AC", "643-add-student-interventions/AC-9")]
    public async Task AddAsync_DoesNotChangeStudentRowsAsync()
    {
        // Arrange (GIVEN) a student with personal data and a profile with its own free-text lists
        using AppDbContext context = CreateContext();
        DateTime modifiedAt = new(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc);
        context.Students.Add(new StudentEntity
        {
            Id = 1,
            Uuid = "uuid-1",
            Name = "Ana Torres",
            Email = "ana@jala.test",
            Audit = new AuditInfo { CreatedBy = "seed", CreatedAt = modifiedAt, ModifiedBy = "seed", ModifiedAt = modifiedAt },
        });
        context.StudentProfiles.Add(new StudentProfile
        {
            StudentId = 1,
            FirstName = "Ana",
            LastName = "Torres",
            IdPassportNumber = "A1",
            IdentifiedRisks = ["Low attendance"],
            SupportReceived = ["Mentoring"],
            Audit = new AuditInfo { CreatedBy = "seed", CreatedAt = modifiedAt, ModifiedBy = "seed", ModifiedAt = modifiedAt },
        });
        await context.SaveChangesAsync();
        StudentInterventionEntryRepository repository = new(context);

        // Act (WHEN) an entry is added to the student
        await repository.AddAsync(NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Financial, new DateOnly(2026, 10, 22)));

        // Assert (THEN) the student and its profile keep their previous values
        StudentEntity student = await context.Students.AsNoTracking().SingleAsync();
        StudentProfile profile = await context.StudentProfiles.AsNoTracking().SingleAsync();
        Assert.Equal("Ana Torres", student.Name);
        Assert.Equal("ana@jala.test", student.Email);
        Assert.Equal("seed", student.Audit.ModifiedBy);
        Assert.Equal(modifiedAt, student.Audit.ModifiedAt);
        Assert.Equal(["Low attendance"], profile.IdentifiedRisks);
        Assert.Equal(["Mentoring"], profile.SupportReceived);
        Assert.Equal("seed", profile.Audit.ModifiedBy);
    }

    [Fact]
    public async Task GetByStudentIdAsync_SameDate_ReturnsMostRecentlyCreatedFirstAsync()
    {
        using AppDbContext context = CreateContext();
        DateOnly date = new(2026, 10, 5);
        context.StudentInterventionEntries.AddRange(
            NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Academic, date, new DateTime(2026, 10, 5, 8, 0, 0, DateTimeKind.Utc)),
            NewEntry(1, StudentInterventionKind.SupportReceived, StudentInterventionType.Referral, date, new DateTime(2026, 10, 5, 9, 0, 0, DateTimeKind.Utc)));
        await context.SaveChangesAsync();
        StudentInterventionEntryRepository repository = new(context);

        IReadOnlyList<StudentInterventionEntry> result = await repository.GetByStudentIdAsync(1);

        Assert.Equal(StudentInterventionType.Referral, result[0].Type);
        Assert.Equal(StudentInterventionType.Academic, result[1].Type);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingEntry_HidesItAndRecordsActorAsync()
    {
        using AppDbContext context = CreateContext();
        StudentInterventionEntry entry = NewEntry(1, StudentInterventionKind.RiskIdentified, StudentInterventionType.Academic, new DateOnly(2026, 10, 1));
        context.StudentInterventionEntries.Add(entry);
        await context.SaveChangesAsync();
        StudentInterventionEntryRepository repository = new(context);

        await repository.SoftDeleteAsync(entry.Id, "sub-admin");

        Assert.Empty(await repository.GetByStudentIdAsync(1));
        Assert.Null(await repository.GetByIdAsync(entry.Id));
        StudentInterventionEntry stored = await context.StudentInterventionEntries.IgnoreQueryFilters().SingleAsync();
        Assert.True(stored.IsDeleted);
        Assert.Equal("sub-admin", stored.Audit.ModifiedBy);
        Assert.NotNull(stored.Audit.ModifiedAt);
        Assert.Equal("sub-ana", stored.Audit.CreatedBy);
    }
}
