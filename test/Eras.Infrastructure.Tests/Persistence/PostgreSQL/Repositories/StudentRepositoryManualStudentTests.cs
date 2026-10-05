using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

/// <summary>
/// Repository behavior behind the "New Student" feature: related-data check,
/// identity update and soft delete.
/// </summary>
public class StudentRepositoryManualStudentTests : RepositoryTestBase
{
    private static StudentRepository CreateRepository(AppDbContext Context) =>
        new(Context, new Mock<IAnswerRiskValidator>().Object);

    private static AuditInfo NewAudit() => new() { CreatedBy = "tester", CreatedAt = DateTime.UtcNow };

    private static async Task<StudentEntity> SeedStudent(AppDbContext Context, string Email, bool IsDeleted = false)
    {
        StudentEntity student = new()
        {
            Uuid = Guid.NewGuid().ToString(),
            Name = "Ana Perez",
            Email = Email,
            IsDeleted = IsDeleted,
            Audit = NewAudit(),
        };
        Context.Students.Add(student);
        await Context.SaveChangesAsync();
        return student;
    }

    private static async Task SeedProfile(AppDbContext Context, int StudentId, string IdPassport, bool IsDeleted = false)
    {
        Context.StudentProfiles.Add(new StudentProfile
        {
            StudentId = StudentId,
            FirstName = "Ana",
            LastName = "Perez",
            IdPassportNumber = IdPassport,
            IsDeleted = IsDeleted,
            Audit = NewAudit(),
        });
        await Context.SaveChangesAsync();
    }

    [Fact]
    public async Task HasRelatedDataAsync_StudentWithoutAnswersOrAssessments_ReturnsFalse()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "a@b.co");

        Assert.False(await CreateRepository(context).HasRelatedDataAsync(student.Id));
    }

    [Fact]
    public async Task HasRelatedDataAsync_StudentWithAPollInstance_ReturnsTrue()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "a@b.co");
        context.PollInstances.Add(new PollInstanceEntity
        {
            Uuid = "poll-instance",
            StudentId = student.Id,
            Audit = NewAudit(),
        });
        await context.SaveChangesAsync();

        Assert.True(await CreateRepository(context).HasRelatedDataAsync(student.Id));
    }

    [Fact]
    public async Task HasRelatedDataAsync_StudentInAnAssessment_ReturnsTrue()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "a@b.co");
        context.Set<Assessment>().Add(new Assessment
        {
            CreatedBy = "tester",
            Service = "Speech",
            StudentIds = [student.Id],
            Status = default,
        });
        await context.SaveChangesAsync();

        Assert.True(await CreateRepository(context).HasRelatedDataAsync(student.Id));
    }

    [Fact]
    public async Task UpdateIdentityAsync_ChangesNameEmailAndAudit()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "old@b.co");

        await CreateRepository(context).UpdateIdentityAsync(student.Id, "Ana María Pérez", "new@b.co", "editor");

        StudentEntity updated = await context.Students.AsNoTracking().SingleAsync(S => S.Id == student.Id);
        Assert.Equal("Ana María Pérez", updated.Name);
        Assert.Equal("new@b.co", updated.Email);
        Assert.Equal("editor", updated.Audit.ModifiedBy);
        Assert.NotNull(updated.Audit.ModifiedAt);
    }

    [Fact]
    public async Task SoftDeleteAsync_FlagsStudentAndProfileInsteadOfRemovingThem()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "a@b.co");
        await SeedProfile(context, student.Id, "AB-1");

        await CreateRepository(context).SoftDeleteAsync(student.Id, "deleter");

        StudentEntity storedStudent = await context.Students.IgnoreQueryFilters().SingleAsync(S => S.Id == student.Id);
        StudentProfile storedProfile = await context.StudentProfiles.IgnoreQueryFilters().SingleAsync();
        Assert.True(storedStudent.IsDeleted);
        Assert.Equal("deleter", storedStudent.Audit.ModifiedBy);
        Assert.True(storedProfile.IsDeleted);
        Assert.Equal("deleter", storedProfile.Audit.ModifiedBy);
    }

    [Fact]
    public async Task SoftDeleteAsync_StudentWithoutProfile_StillFlagsTheStudent()
    {
        using AppDbContext context = CreateContext();
        StudentEntity student = await SeedStudent(context, "a@b.co");

        await CreateRepository(context).SoftDeleteAsync(student.Id, "deleter");

        Assert.True((await context.Students.IgnoreQueryFilters().SingleAsync()).IsDeleted);
    }

    [Fact]
    public async Task SoftDeletedStudent_IsHiddenFromEveryLookup()
    {
        using AppDbContext context = CreateContext();
        StudentEntity kept = await SeedStudent(context, "kept@b.co");
        StudentEntity deleted = await SeedStudent(context, "gone@b.co", IsDeleted: true);
        StudentRepository repository = CreateRepository(context);

        Assert.Null(await repository.GetByIdAsync(deleted.Id));
        Assert.Null(await repository.GetByEmailAsync("gone@b.co"));
        Assert.NotNull(await repository.GetByIdAsync(kept.Id));
        Assert.Equal(1, await repository.CountAsync());
    }

    [Fact]
    public async Task SoftDeletedProfile_NoLongerReservesItsIdPassportNumber()
    {
        using AppDbContext context = CreateContext();
        StudentEntity gone = await SeedStudent(context, "gone@b.co", IsDeleted: true);
        await SeedProfile(context, gone.Id, "AB-1", IsDeleted: true);
        StudentProfileRepository profiles = new(context);

        Assert.False(await profiles.ExistsByIdPassportNumberAsync("AB-1"));
        Assert.Null(await profiles.GetByStudentIdAsync(gone.Id));
    }
}
