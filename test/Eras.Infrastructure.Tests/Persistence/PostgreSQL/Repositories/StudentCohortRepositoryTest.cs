using Eras.Application.Utils;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories;

public class StudentCohortRepositoryTest
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByCohortIdAndStudentIdAsync_ShouldReturnStudent_WhenExistsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Email = "stu@mail.com",
            Name = "Student 1"
        };

        context.Students.Add(student);

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10,
            Student = student
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetByCohortIdAndStudentIdAsync(
            CohortId: 10,
            StudentId: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("student-uuid", result.Uuid);
        Assert.Equal("Student 1", result.Name);
    }

    [Fact]
    public async Task GetByCohortIdAndStudentIdAsync_ShouldReturnNull_WhenStudentDoesNotExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetByCohortIdAndStudentIdAsync(
            CohortId: 10,
            StudentId: 999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCohortIdAndStudentIdAsync_ShouldReturnNull_WhenCohortDoesNotMatchAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Email = "stu@mail.com",
            Name = "Student 1"
        };

        context.Students.Add(student);

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10,
            Student = student
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetByCohortIdAndStudentIdAsync(
            CohortId: 999,
            StudentId: 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllStudentsByCohortIdAsync_ShouldReturnStudentsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var student1 = new StudentEntity
        {
            Id = 1,
            Uuid = "student-1",
            Email = "stu@mail.com",
            Name = "Student 1",
        };

        var student2 = new StudentEntity
        {
            Id = 2,
            Uuid = "student-2",
            Email = "stu@mail.com",
            Name = "Student 2",
        };

        context.Students.AddRange(student1, student2);

        context.StudentCohorts.AddRange(
            new StudentCohortJoin
            {
                StudentId = 1,
                CohortId = 10,
                Student = student1
            },
            new StudentCohortJoin
            {
                StudentId = 2,
                CohortId = 10,
                Student = student2
            });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetAllStudentsByCohortIdAsync(10);

        // Assert
        Assert.NotNull(result);

        var students = result.ToList();

        Assert.Equal(2, students.Count);
        Assert.Contains(students, S => S.Id == 1);
        Assert.Contains(students, S => S.Id == 2);
    }

    [Fact]
    public async Task GetAllStudentsByCohortIdAsync_ShouldReturnOnlyStudentsFromRequestedCohortAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var student1 = new StudentEntity
        {
            Id = 1,
            Uuid = "student-1",
            Name = "Student 1",
            Email = "stu@mail.com"
        };

        var student2 = new StudentEntity
        {
            Id = 2,
            Uuid = "student-2",
            Name = "Student 2",
            Email = "stu@mail.com"
        };

        context.Students.AddRange(student1, student2);

        context.StudentCohorts.AddRange(
            new StudentCohortJoin
            {
                StudentId = 1,
                CohortId = 10,
                Student = student1
            },
            new StudentCohortJoin
            {
                StudentId = 2,
                CohortId = 20,
                Student = student2
            });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetAllStudentsByCohortIdAsync(10);

        // Assert
        var students = result!.ToList();

        Assert.Single(students);
        Assert.Equal(1, students[0].Id);
    }

    [Fact]
    public async Task GetAllStudentsByCohortIdAsync_ShouldReturnEmpty_WhenCohortHasNoStudentsAsync()
    {
        // Arrange
        await using var context = CreateContext();

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        // Act
        var result = await repository.GetAllStudentsByCohortIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetCohortsSummaryAsync_ShouldReturnStudentAndCohortSummaryAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var cohort = new CohortEntity
        {
            Id = 10,
            Name = "Cohort 1",
            CourseCode = "123"
        };

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Name = "Student 1",
            Email = "stu@mail.com"
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Student = student,
            FinishedAt = new DateTime(2026, 1, 15)
        };

        var pollVariable = new PollVariableJoin
        {
            Id = 200,
            PollId = 50,
            VariableId = 300
        };

        var answer1 = new AnswerEntity
        {
            Id = 1,
            PollInstanceId = 100,
            PollVariableId = 200,
            RiskLevel = 2,
            PollVariable = pollVariable
        };

        var answer2 = new AnswerEntity
        {
            Id = 2,
            PollInstanceId = 100,
            PollVariableId = 200,
            RiskLevel = 4,
            PollVariable = pollVariable
        };

        pollInstance.Answers = new List<AnswerEntity>
        {
            answer1,
            answer2
        };

        student.PollInstances = new List<PollInstanceEntity>
        {
            pollInstance
        };

        context.Cohorts.Add(cohort);
        context.Students.Add(student);
        context.PollInstances.Add(pollInstance);
        context.PollVariables.Add(pollVariable);
        context.Answers.AddRange(answer1, answer2);

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10,
            Student = student,
            Cohort = cohort
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        var pagination = new Pagination
        {
            Page = 0,
            PageSize = 10
        };

        // Act
        var result = await repository.GetCohortsSummaryAsync(pagination);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(1, result.StudentCount);
        Assert.Equal(1, result.CohortCount);

        var summary = result.Summary.ToList();

        Assert.Single(summary);

        Assert.Equal("student-uuid", summary[0].StudentUuid);
        Assert.Equal("Student 1", summary[0].StudentName);
        Assert.Equal(10, summary[0].CohortId);
        Assert.Equal("Cohort 1", summary[0].CohortName);

        // (2 + 4) / 2 = 3
        Assert.Equal(3, summary[0].PollinstancesAverage);

        Assert.Equal(1, summary[0].PollinstancesCount);
    }

    [Fact]
    public async Task GetCohortsSummaryAsync_ShouldReturnZeroPollInstances_WhenStudentHasNoPollInstancesAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var cohort = new CohortEntity
        {
            Id = 10,
            Name = "Cohort 1",
            CourseCode = "123"
        };

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Name = "Student 1",
            Email = "stu@mail.com"
        };

        context.Cohorts.Add(cohort);
        context.Students.Add(student);

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10,
            Student = student,
            Cohort = cohort
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        var pagination = new Pagination
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await repository.GetCohortsSummaryAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.StudentCount);
        Assert.Equal(1, result.CohortCount);
        Assert.Empty(result.Summary);
    }

    [Fact]
    public async Task GetCohortsSummaryAsync_ShouldIgnorePollInstanceOutsideDateRangeAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var cohort = new CohortEntity
        {
            Id = 10,
            Name = "Cohort 1",
            CourseCode = "123"
        };

        var student = new StudentEntity
        {
            Id = 1,
            Uuid = "student-uuid",
            Name = "Student 1",
            Email = "stu@mail.com"
        };

        var pollInstance = new PollInstanceEntity
        {
            Id = 100,
            StudentId = 1,
            Student = student,
            FinishedAt = new DateTime(2025, 1, 1)
        };

        context.Cohorts.Add(cohort);
        context.Students.Add(student);
        context.PollInstances.Add(pollInstance);

        context.StudentCohorts.Add(new StudentCohortJoin
        {
            StudentId = 1,
            CohortId = 10,
            Student = student,
            Cohort = cohort
        });

        await context.SaveChangesAsync();

        var repository = new StudentCohortRepository(context);

        var pagination = new Pagination
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await repository.GetCohortsSummaryAsync(
            pagination,
            startDate: new DateTime(2026, 1, 1),
            endDate: new DateTime(2026, 12, 31));

        // Assert
        Assert.Equal(1, result.StudentCount);
        Assert.Equal(1, result.CohortCount);
        Assert.Empty(result.Summary);
    }

    [Fact]
    public async Task GetCohortsSummaryAsync_ShouldReturnEmptySummary_WhenNoStudentsExistAsync()
    {
        // Arrange
        await using var context = CreateContext();

        var repository = new StudentCohortRepository(context);

        var pagination = new Pagination { Page = 1, PageSize = 10 };

        // Act
        var result = await repository.GetCohortsSummaryAsync(pagination);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.StudentCount);
        Assert.Equal(0, result.CohortCount);
        Assert.Empty(result.Summary);
    }
}
