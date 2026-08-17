using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{
    public class TestDbContext : AppDbContext
    {
        public TestDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ErasCalculationsByPollEntity>().HasKey(e => new { e.StudentId, e.PollVariableId, e.PollInstanceId });
        }
    }

    public class StudentRepositoryTest : RepositoryTestBase
    {
        private readonly AppDbContext _context;
        private readonly StudentRepository _repository;
        private readonly Mock<IAnswerRiskValidator> _mockValidator;

        public StudentRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new TestDbContext(options);
            _context.Database.EnsureCreated();
            
            _mockValidator = new Mock<IAnswerRiskValidator>();
            _mockValidator.Setup(v => v.IsValidAnswer(It.IsAny<string>())).Returns(true);
            
            _repository = new StudentRepository(_context, _mockValidator.Object);
        }

        private static Mock<DbSet<ErasCalculationsByPollEntity>> CreateMockErasCalculations(IEnumerable<ErasCalculationsByPollEntity> Data)
        {
            var queryable = new TestAsyncEnumerable<ErasCalculationsByPollEntity>(Data);
            var dbSet = new Mock<DbSet<ErasCalculationsByPollEntity>>();

            dbSet
                .As<IAsyncEnumerable<ErasCalculationsByPollEntity>>()
                .Setup(X => X.GetAsyncEnumerator(
                    It.IsAny<CancellationToken>()))
                .Returns(() => queryable.GetAsyncEnumerator());

            dbSet
                .As<IQueryable<ErasCalculationsByPollEntity>>()
                .Setup(X => X.Provider)
                .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Provider);

            dbSet
                .As<IQueryable<ErasCalculationsByPollEntity>>()
                .Setup(X => X.Expression)
                .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).Expression);

            dbSet
                .As<IQueryable<ErasCalculationsByPollEntity>>()
                .Setup(X => X.ElementType)
                .Returns(((IQueryable<ErasCalculationsByPollEntity>)queryable).ElementType);

            dbSet
                .As<IQueryable<ErasCalculationsByPollEntity>>()
                .Setup(X => X.GetEnumerator())
                .Returns(() => queryable.AsEnumerable().GetEnumerator());
            return dbSet;
        }

        private static List<ErasCalculationsByPollEntity> CreateErasCalculations()
        {
            return [
                new ErasCalculationsByPollEntity
                {
                    StudentId = 1,
                    StudentName = "Juan",
                    StudentEmail = "juan@test.com",
                    CohortId = 10,
                    PollUuid = "poll-1",
                    PollVersion = 2,
                    AnswerRisk = 4,
                    PollVariableId = 1,
                    PollInstanceId = 1,
                    PollId = 1,
                    CohortName = "Cohort A",
                    ComponentName = "Engagement",
                    Question = "question2",
                    AnswerText = "Response",
                },
                new ErasCalculationsByPollEntity
                {
                    StudentId = 1,
                    StudentName = "Juan",
                    StudentEmail = "juan@test.com",
                    CohortId = 10,
                    PollUuid = "poll-1",
                    PollVersion = 2,
                    AnswerRisk = 6,
                    PollVariableId = 2,
                    PollInstanceId = 2,
                    PollId = 1,
                    CohortName = "Cohort A",
                    ComponentName = "Attendance",
                    Question = "question3",
                    AnswerText = "Response",
                },
                new ErasCalculationsByPollEntity
                {
                    StudentId = 1,
                    StudentName = "Juan",
                    StudentEmail = "juan@test.com",
                    CohortId = 10,
                    PollUuid = "poll-1",
                    PollVersion = 1,
                    AnswerRisk = 100,
                    PollVariableId = 3,
                    PollInstanceId = 3,
                    PollId = 1,
                    CohortName = "Cohort A",
                    ComponentName = "Engagement",
                    Question = "question5",
                    AnswerText = "Response"
                }
            ];
        }

        [Fact]
        public async Task BasicGetMethods_ShouldReturnCorrectDataAsync()
        {
            var student = new StudentEntity { Id = 10, Name = "Juan", Uuid = "u-1", Email = "j@j.com", IsImported = false };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            Assert.NotNull(await _repository.GetByNameAsync("Juan"));
            Assert.NotNull(await _repository.GetByUuidAsync("u-1"));
            Assert.NotNull(await _repository.GetByEmailAsync("j@j.com"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSuccessfullyAsync()
        {
            var entity = new StudentEntity { Id = 1, Name = "Old", Email = "o@t.com", Uuid = "u1", IsImported = false };
            _context.Students.Add(entity);
            await _context.SaveChangesAsync();

            var domain = new Student { 
                Id = 1, 
                Name = "New", 
                Audit = new AuditInfo(),
                StudentDetail = new StudentDetail { AvgScore = 5 } 
            };
            
            var result = await _repository.UpdateAsync(domain);
            Assert.Equal("New", result.Name);
        }

        [Fact]
        public async Task GetStudentHeatMapDetailsByComponent_ShouldHandleComplexJoinsAsync()
        {
            _context.Components.Add(new ComponentEntity { Id = 1, Name = "Salud" });
            _context.Variables.Add(new VariableEntity { Id = 1, ComponentId = 1 });
            _context.PollVariables.Add(new PollVariableJoin { Id = 1, VariableId = 1 });
            _context.Students.Add(new StudentEntity { Id = 1, Name = "Pepe", Email = "p@p.com", Uuid = "u1", IsImported = false });
            _context.StudentDetails.Add(new StudentDetailEntity { Id = 1, StudentId = 1 });
            _context.PollInstances.Add(new PollInstanceEntity { Id = 1, StudentId = 1 });
            _context.Answers.Add(new AnswerEntity { Id = 1, PollInstanceId = 1, PollVariableId = 1, RiskLevel = 5 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetStudentHeatMapDetailsByComponent("Salud", 5);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetStudentHeatMapDetailsByCohort_ShouldHandleInvalidIdAsync()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetStudentHeatMapDetailsByCohort("invalid", 5));

            _context.Cohorts.Add(new CohortEntity { Id = 1, Name = "C1", CourseCode = "CODE123" });
            _context.StudentCohorts.Add(new StudentCohortJoin { Id = 1, StudentId = 1, CohortId = 1 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetStudentHeatMapDetailsByCohort("1", 5);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_CoverageAsync()
        {
            var pagination = new Pagination { Page = 1, PageSize = 10 };
            _context.Polls.Add(new PollEntity { Id = 1, Uuid = "p1", LastVersion = 1 });
            
            _context.ErasCalculationsByPoll.Add(new ErasCalculationsByPollEntity 
            { 
                StudentId = 1, 
                CohortId = 1, 
                PollUuid = "p1", 
                AnswerRisk = 4, 
                PollVersion = 1,
                StudentName = "Test",
                StudentEmail = "t@t.com",
                ComponentName = "Salud",
                Question = "Q1",
                AnswerText = "OK",
                CohortName = "C1",
                PollVariableId = 1,
                PollInstanceId = 1 
            });
            await _context.SaveChangesAsync();

            var res = await _repository.GetStudentAverageRiskByCohortsAsync(pagination, new List<int>{1}, "p1", true, null);
            Assert.Equal(1, res.Count);
        }

        [Fact]
        public async Task GetAverageRiskByStudentIdsAsync_FilteringAndGroupingAsync()
        {
            _mockValidator.Setup(v => v.IsValidAnswer("OK")).Returns(true);
            _mockValidator.Setup(v => v.IsValidAnswer("BAD")).Returns(false);

            _context.ErasCalculationsByPoll.AddRange(
                new ErasCalculationsByPollEntity 
                { 
                    StudentId = 1, ComponentId = 1, AnswerRisk = 10, AnswerText = "OK",
                    StudentName = "T1", StudentEmail = "t1@t.com", ComponentName = "C", 
                    Question = "Q", CohortName = "CH", PollUuid = "P", CohortId = 1,
                    PollVariableId = 1, PollInstanceId = 1
                },
                new ErasCalculationsByPollEntity 
                { 
                    StudentId = 1, ComponentId = 2, AnswerRisk = 0, AnswerText = "BAD",
                    StudentName = "T1", StudentEmail = "t1@t.com", ComponentName = "C", 
                    Question = "Q", CohortName = "CH", PollUuid = "P", CohortId = 1,
                    PollVariableId = 2, PollInstanceId = 2
                }
            );
            await _context.SaveChangesAsync();

            var result = await _repository.GetAverageRiskByStudentIdsAsync(new List<int> { 1 });
            Assert.Equal(10, result[1]);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnStudentsWithMatchingIdsAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Juan",
                    Uuid = "uuid-1",
                    Email = "juan@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "uuid-2",
                    Email = "pedro@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 3,
                    Name = "Maria",
                    Uuid = "uuid-3",
                    Email = "maria@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdsAsync(new[] { 1, 3 });

            // Assert
            var students = result.ToList();

            Assert.Equal(2, students.Count);
            Assert.Contains(students, s => s.Id == 1 && s.Name == "Juan");
            Assert.Contains(students, s => s.Id == 3 && s.Name == "Maria");
            Assert.DoesNotContain(students, s => s.Id == 2);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmpty_WhenIdsDoNotExistAsync()
        {
            // Arrange
            _context.Students.Add(new StudentEntity
            {
                Id = 1,
                Name = "Juan",
                Uuid = "uuid-1",
                Email = "juan@test.com",
                IsImported = false
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdsAsync(new[] { 999, 1000 });

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmpty_WhenIdsAreEmptyAsync()
        {
            // Arrange
            _context.Students.Add(new StudentEntity
            {
                Id = 1,
                Name = "Juan",
                Uuid = "uuid-1",
                Email = "juan@test.com",
                IsImported = false
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdsAsync(Array.Empty<int>());

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldNotReturnStudentsNotIncludedInIdsAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Juan",
                    Uuid = "uuid-1",
                    Email = "juan@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "uuid-2",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdsAsync(new[] { 2 });

            // Assert
            var student = Assert.Single(result);

            Assert.Equal(2, student.Id);
            Assert.Equal("Pedro", student.Name);
        }

        [Fact]
        public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldReturnStudentsForPollAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Juan",
                    Uuid = "student-1",
                    Email = "juan@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "student-2",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            _context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1,
                    StudentId = 1,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow
                },
                new PollInstanceEntity
                {
                    Id = 2,
                    StudentId = 2,
                    Uuid = "different-poll",
                    FinishedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllStudentsByPollUuidAndDaysQuery(1, 10, "poll-1");

            // Assert
            Assert.Equal(1, result.TotalCount);

            var students = result.Students.ToList();

            Assert.Single(students);
            Assert.Equal(1, students[0].Id);
            Assert.Equal("Juan", students[0].Name);
        }

        [Fact]
        public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldReturnEmpty_WhenPollDoesNotExistAsync()
        {
            // Arrange
            _context.Students.Add(new StudentEntity
            {
                Id = 1,
                Name = "Juan",
                Uuid = "student-1",
                Email = "juan@test.com",
                IsImported = false
            });

            _context.PollInstances.Add(new PollInstanceEntity
            {
                Id = 1,
                StudentId = 1,
                Uuid = "poll-1",
                FinishedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllStudentsByPollUuidAndDaysQuery(1, 10, "unknown-poll");

            // Assert
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Students);
        }

        [Fact]
        public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldReturnAllStudents_WhenDaysIsNullAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Recent",
                    Uuid = "student-1",
                    Email = "recent@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Old",
                    Uuid = "student-2",
                    Email = "old@test.com",
                    IsImported = false
                });

            _context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1,
                    StudentId = 1,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow
                },
                new PollInstanceEntity
                {
                    Id = 2,
                    StudentId = 2,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow.AddDays(-100)
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllStudentsByPollUuidAndDaysQuery(1, 10, "poll-1", null);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Students.Count());
        }

        //[Fact]
        //public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldReturnAllStudents_WhenDaysIsZero()
        //{
        //    // Arrange
        //    _context.Students.AddRange(
        //        new StudentEntity
        //        {
        //            Id = 1,
        //            Name = "Recent",
        //            Uuid = "student-1",
        //            Email = "recent@test.com",
        //            IsImported = false
        //        },
        //        new StudentEntity
        //        {
        //            Id = 2,
        //            Name = "Old",
        //            Uuid = "student-2",
        //            Email = "old@test.com",
        //            IsImported = false
        //        });

        //    _context.PollInstances.AddRange(
        //        new PollInstanceEntity
        //        {
        //            Id = 1,
        //            StudentId = 1,
        //            Uuid = "poll-1",
        //            FinishedAt = DateTime.UtcNow
        //        },
        //        new PollInstanceEntity
        //        {
        //            Id = 2,
        //            StudentId = 2,
        //            Uuid = "poll-1",
        //            FinishedAt = DateTime.UtcNow.AddDays(-100)
        //        });

        //    await _context.SaveChangesAsync();

        //    // Act
        //    var result =
        //        await _repository.GetAllStudentsByPollUuidAndDaysQuery(
        //            1,
        //            10,
        //            "poll-1",
        //            0);

        //    // Assert
        //    Assert.Equal(2, result.TotalCount);
        //    Assert.Equal(2, result.Students.Count());
        //}

        [Fact]
        public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldFilterByDaysAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Recent",
                    Uuid = "student-1",
                    Email = "recent@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Old",
                    Uuid = "student-2",
                    Email = "old@test.com",
                    IsImported = false
                });

            _context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1,
                    StudentId = 1,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow.AddDays(-2)
                },
                new PollInstanceEntity
                {
                    Id = 2,
                    StudentId = 2,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow.AddDays(-30)
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllStudentsByPollUuidAndDaysQuery(1, 10, "poll-1", 7);

            // Assert
            Assert.Equal(1, result.TotalCount);

            var student = Assert.Single(result.Students);

            Assert.Equal(1, student.Id);
            Assert.Equal("Recent", student.Name);
        }

        [Fact]
        public async Task GetAllStudentsByPollUuidAndDaysQuery_ShouldApplyPaginationAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Student 1",
                    Uuid = "student-1",
                    Email = "1@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Student 2",
                    Uuid = "student-2",
                    Email = "2@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 3,
                    Name = "Student 3",
                    Uuid = "student-3",
                    Email = "3@test.com",
                    IsImported = false
                });

            _context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1,
                    StudentId = 1,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow
                },
                new PollInstanceEntity
                {
                    Id = 2,
                    StudentId = 2,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow
                },
                new PollInstanceEntity
                {
                    Id = 3,
                    StudentId = 3,
                    Uuid = "poll-1",
                    FinishedAt = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();

            // Act
            var result =
                await _repository.GetAllStudentsByPollUuidAndDaysQuery(
                    2,
                    1,
                    "poll-1");

            // Assert
            Assert.Equal(3, result.TotalCount);

            var students = result.Students.ToList();

            Assert.Single(students);
            Assert.Equal(2, students[0].Id);
        }

        //[Fact]
        //public async Task GetStudentAverageRiskByCohortsAsync_ShouldReturnLastVersionResultsAsync()
        //{
        //    // Arrange
        //    _context.Polls.Add(new PollEntity
        //    {
        //        Id = 1,
        //        Uuid = "poll-1",
        //        LastVersion = 2
        //    });

        //    var calculations = CreateErasCalculations();
        //    var calculationsDbSet = CreateMockErasCalculations(calculations);
        //    _context.ErasCalculationsByPoll = calculationsDbSet.Object;
        //    await _context.SaveChangesAsync();

        //    var pagination = new Pagination
        //    {
        //        Page = 1,
        //        PageSize = 10
        //    };

        //    // Act
        //    var result = await _repository.GetStudentAverageRiskByCohortsAsync(pagination, new List<int> { 10 }, "poll-1", true, null);

        //    // Assert
        //    Assert.Equal(1, result.Count);

        //    var student = Assert.Single(result.Items);

        //    Assert.Equal(1, student.StudentId);
        //    Assert.Equal("Juan", student.StudentName);
        //    Assert.Equal("juan@test.com", student.Email);
        //    Assert.Equal(5, student.AvgRiskLevel);
        //}

        //[Fact]
        //public async Task GetStudentAverageRiskByCohortsAsync_ShouldReturnPreviousVersionResults_WhenLastVersionIsFalseAsync()
        //{
        //    // Arrange
        //    _context.Polls.Add(new PollEntity
        //    {
        //        Id = 1,
        //        Uuid = "poll-1",
        //        LastVersion = 2
        //    });

        //    var calculations = CreateErasCalculations();
        //    var calculationsDbSet = CreateMockErasCalculations(calculations);
        //    _context.ErasCalculationsByPoll = calculationsDbSet.Object;
        //    await _context.SaveChangesAsync();

        //    var pagination = new Pagination
        //    {
        //        Page = 1,
        //        PageSize = 10
        //    };

        //    // Act
        //    var result =
        //        await _repository.GetStudentAverageRiskByCohortsAsync(
        //            pagination,
        //            new List<int> { 10 },
        //            "poll-1",
        //            false,
        //            null);

        //    // Assert
        //    Assert.Equal(1, result.Count);

        //    var student = Assert.Single(result.Items);

        //    Assert.Equal(1, student.StudentId);
        //    Assert.Equal(8, student.AvgRiskLevel);
        //}

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_ShouldFilterByCohortAsync()
        {
            // Arrange
            _context.Polls.Add(new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 1
            });

            var calculations = CreateErasCalculations();
            var calculationsDbSet = CreateMockErasCalculations(calculations);
            _context.ErasCalculationsByPoll = calculationsDbSet.Object;
            await _context.SaveChangesAsync();

            var pagination = new Pagination
            {
                Page = 0,
                PageSize = 10
            };

            // Act
            var result =
                await _repository.GetStudentAverageRiskByCohortsAsync(
                    pagination,
                    new List<int> { 10 },
                    "poll-1",
                    true,
                    null);

            // Assert
            Assert.Equal(1, result.Count);

            var student = Assert.Single(result.Items);

            Assert.Equal(1, student.StudentId);
            Assert.Equal("Juan", student.StudentName);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_ShouldReturnEmpty_WhenNoStudentsMatchAsync()
        {
            // Arrange
            _context.Polls.Add(new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 1
            });

            await _context.SaveChangesAsync();

            var pagination = new Pagination
            {
                Page = 1,
                PageSize = 10
            };

            // Act
            var result =
                await _repository.GetStudentAverageRiskByCohortsAsync(
                    pagination,
                    new List<int> { 10 },
                    "poll-1",
                    true,
                    null);

            // Assert
            Assert.Equal(0, result.Count);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_ShouldFallBackToDefaultQuery_WhenEvaluationDoesNotExistAsync()
        {
            // Arrange
            _context.Polls.Add(new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 2
            });

            var calculations = CreateErasCalculations();
            var calculationsDbSet = CreateMockErasCalculations(calculations);
            _context.ErasCalculationsByPoll = calculationsDbSet.Object;
            await _context.SaveChangesAsync();

            var pagination = new Pagination
            {
                Page = 0,
                PageSize = 10
            };

            // Act
            var result =
                await _repository.GetStudentAverageRiskByCohortsAsync(
                    pagination,
                    new List<int> { 10 },
                    "poll-1",
                    true,
                    999);

            // Assert
            Assert.Equal(1, result.Count);

            var student = Assert.Single(result.Items);

            Assert.Equal(1, student.StudentId);
            Assert.Equal(5, student.AvgRiskLevel);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_ShouldGroupMultipleAnswersForStudentAsync()
        {
            // Arrange
            _context.Polls.Add(new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 1
            });

            var calculations = CreateErasCalculations();
            var calculationsDbSet = CreateMockErasCalculations(calculations);
            _context.ErasCalculationsByPoll = calculationsDbSet.Object;
            await _context.SaveChangesAsync();

            var pagination = new Pagination
            {
                Page = 0,
                PageSize = 10
            };

            // Act
            var result =
                await _repository.GetStudentAverageRiskByCohortsAsync(pagination, new List<int> { 10 }, "poll-1", true, null);

            // Assert
            var student = Assert.Single(result.Items);

            Assert.Equal(1, student.StudentId);
            Assert.Equal(100, student.AvgRiskLevel);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_ShouldApplyPaginationAsync()
        {
            // Arrange
            _context.Polls.Add(new PollEntity
            {
                Id = 1,
                Uuid = "poll-1",
                LastVersion = 1
            });

            var calculations = CreateErasCalculations();
            var calculationsDbSet = CreateMockErasCalculations(calculations);
            _context.ErasCalculationsByPoll = calculationsDbSet.Object;
            await _context.SaveChangesAsync();

            var pagination = new Pagination
            {
                Page = 0,
                PageSize = 1
            };

            // Act
            var result =
                await _repository.GetStudentAverageRiskByCohortsAsync(pagination, new List<int> { 10 }, "poll-1",true, null);

            // Assert
            Assert.Equal(1, result.Count);

            var student = Assert.Single(result.Items);

            Assert.Equal(1, student.StudentId);
            Assert.Equal("Juan", student.StudentName);
        }

        [Fact]
        public async Task GetPagedAsyncWithJoins_ShouldReturnStudentsAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Juan",
                    Uuid = "uuid-1",
                    Email = "juan@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "uuid-2",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result =
                await _repository.GetPagedAsyncWithJoins(1, 10);

            // Assert
            var students = result.ToList();

            Assert.Equal(2, students.Count);
        }

        [Fact]
        public async Task GetPagedAsyncWithJoins_ShouldOrderStudentsByNameAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Zoe",
                    Uuid = "uuid-1",
                    Email = "zoe@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Ana",
                    Uuid = "uuid-2",
                    Email = "ana@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 3,
                    Name = "Pedro",
                    Uuid = "uuid-3",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result =
                await _repository.GetPagedAsyncWithJoins(1, 10);

            // Assert
            var students = result.ToList();

            Assert.Equal(3, students.Count);
            Assert.Equal("Ana", students[0].Name);
            Assert.Equal("Pedro", students[1].Name);
            Assert.Equal("Zoe", students[2].Name);
        }

        [Fact]
        public async Task GetPagedAsyncWithJoins_ShouldApplyPaginationAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Ana",
                    Uuid = "uuid-1",
                    Email = "ana@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "uuid-2",
                    Email = "pedro@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 3,
                    Name = "Zoe",
                    Uuid = "uuid-3",
                    Email = "zoe@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result =
                await _repository.GetPagedAsyncWithJoins(2, 1);

            // Assert
            var students = result.ToList();

            Assert.Single(students);
            Assert.Equal("Pedro", students[0].Name);
        }

        [Fact]
        public async Task GetPagedAsyncWithJoins_ShouldReturnEmpty_WhenNoStudentsExistAsync()
        {
            // Arrange
            // Context intentionally empty.

            // Act
            var result =
                await _repository.GetPagedAsyncWithJoins(1, 10);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLightAsync_ShouldReturnAllStudentsAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Juan",
                    Uuid = "uuid-1",
                    Email = "juan@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Pedro",
                    Uuid = "uuid-2",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllLightAsync();

            // Assert
            var students = result.ToList();

            Assert.Equal(2, students.Count);

            Assert.Contains(students, s =>
                s.Id == 1 &&
                s.Name == "Juan");

            Assert.Contains(students, s =>
                s.Id == 2 &&
                s.Name == "Pedro");
        }

        [Fact]
        public async Task GetAllLightAsync_ShouldOrderByNameAsync()
        {
            // Arrange
            _context.Students.AddRange(
                new StudentEntity
                {
                    Id = 1,
                    Name = "Zoe",
                    Uuid = "uuid-1",
                    Email = "zoe@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 2,
                    Name = "Ana",
                    Uuid = "uuid-2",
                    Email = "ana@test.com",
                    IsImported = false
                },
                new StudentEntity
                {
                    Id = 3,
                    Name = "Pedro",
                    Uuid = "uuid-3",
                    Email = "pedro@test.com",
                    IsImported = false
                });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllLightAsync();

            // Assert
            var students = result.ToList();

            Assert.Equal(3, students.Count);
            Assert.Equal("Ana", students[0].Name);
            Assert.Equal("Pedro", students[1].Name);
            Assert.Equal("Zoe", students[2].Name);
        }

        [Fact]
        public async Task GetAllLightAsync_ShouldReturnEmpty_WhenNoStudentsExistAsync()
        {
            // Act
            var result = await _repository.GetAllLightAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllLightAsync_ShouldReturnOnlyIdAndNameAsync()
        {
            // Arrange
            _context.Students.Add(new StudentEntity
            {
                Id = 10,
                Name = "Juan",
                Uuid = "secret-uuid",
                Email = "juan@test.com",
                IsImported = true
            });

            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllLightAsync();

            // Assert
            var student = Assert.Single(result);

            Assert.Equal(10, student.Id);
            Assert.Equal("Juan", student.Name);
        }
    }
}