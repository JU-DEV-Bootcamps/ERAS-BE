using Eras.Application.Models.Consolidator;
using Eras.Domain.Common;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

using Microsoft.EntityFrameworkCore;

using MockQueryable.Moq;

using Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{
    public class PollInstanceRepositoryTest : RepositoryTestBase
    {
        protected Mock<AppDbContext> _mockContext;
        private PollInstanceRepository? _repository;

        public PollInstanceRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"mock-context-{Guid.NewGuid()}")
                .Options;

            _mockContext = new Mock<AppDbContext>(options);
        }

        [Fact]
        public void GetByLastDays_Should_Return_Only_Instances_Within_Range_And_Version()
        {
            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", FinishedAt = DateTime.UtcNow, StudentId = 1, LastVersion = 1 },
                new PollInstanceEntity { Id = 2, Uuid = "poll-Uuid", FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 1, LastVersion = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = _repository.GetByLastDays(10, true, "poll-Uuid").Result;

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void GetByLastDays_Should_Throw_When_Poll_Not_Found()
        {
            var pollData = new List<PollEntity>().AsQueryable().BuildMockDbSet();
            var pollInstanceData = new List<PollInstanceEntity>().AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _repository.GetByLastDays(10, true, "missing-Uuid"));
        }

        [Fact]
        public void GetByLastDays_ShouldReturn_OnlyInstances_Within_Range_And_VersionIsOld()
        {
            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", FinishedAt = DateTime.UtcNow, StudentId = 1, LastVersion = 2 },
                new PollInstanceEntity { Id = 2, Uuid = "poll-Uuid", FinishedAt = DateTime.UtcNow.AddDays(-100), StudentId = 1, LastVersion = 2 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);


            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = _repository.GetByLastDays(10, false, "poll-Uuid").Result;

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDays_WithEvaluationId_Returns_Only_That_Evaluations_StudentsAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            context.Polls.Add(new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 });
            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = 1, CohortId = 100 });
            context.PollInstances.AddRange(
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student },
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 20, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student },
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 30, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student }
            );
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);

            var result = await _repository.GetByCohortIdAndLastDays(
                Page: 1,
                PageSize: 10,
                CohortId: [100],
                Days: null,
                LastVersion: true,
                PollUuid: "poll-Uuid",
                StartDate: DateTime.UtcNow.AddDays(-1),
                EndDate: DateTime.UtcNow.AddDays(1),
                EvaluationId: 10
            );

            Assert.Equal(1, result.Count);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDays_WithoutEvaluationId_Returns_All_Evaluations_For_PollAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            context.Polls.Add(new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 });
            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = 1, CohortId = 100 });
            context.PollInstances.AddRange(
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student },
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 20, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student }
            );
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);

            var result = await _repository.GetByCohortIdAndLastDays(
                Page: 1,
                PageSize: 10,
                CohortId: [100],
                Days: null,
                LastVersion: true,
                PollUuid: "poll-Uuid",
                StartDate: DateTime.UtcNow.AddDays(-1),
                EndDate: DateTime.UtcNow.AddDays(1),
                EvaluationId: null
            );

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDays_Excludes_Students_Outside_CohortAsync()
        {
            using var context = CreateContext();

            var studentInCohort = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student In Cohort", Email = "in-cohort@test.com" };
            var studentOutsideCohort = new StudentEntity { Id = 2, Uuid = "student-2-uuid", Name = "Student Outside Cohort", Email = "outside-cohort@test.com" };

            context.Polls.Add(new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 });
            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = 1, CohortId = 100 });
            context.PollInstances.AddRange(
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = studentInCohort },
                new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 2, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = studentOutsideCohort }
            );
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);

            var result = await _repository.GetByCohortIdAndLastDays(
                Page: 1,
                PageSize: 10,
                CohortId: [100],
                Days: null,
                LastVersion: true,
                PollUuid: "poll-Uuid",
                StartDate: DateTime.UtcNow.AddDays(-1),
                EndDate: DateTime.UtcNow.AddDays(1),
                EvaluationId: 10
            );

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.Items.First().Student!.Id);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDays_WithDaysAndLastVersion_ReturnsRecentLastVersionAsync()
        {
            using var context = CreateContext();

            var now = DateTime.UtcNow;

            var student = new StudentEntity
            { Id = 1, Uuid = "student-1", Name = "Student", Email = "student@test.com"};

            context.Polls.Add(new PollEntity { Uuid = "poll-Uuid", LastVersion = 2 });

            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = 1, CohortId = 100 });

            context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = now.AddDays(-2), LastVersion = 2,Student = student
                },
                new PollInstanceEntity
                {
                    Id = 2, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = now.AddDays(-10), LastVersion = 2, Student = student
                },
                new PollInstanceEntity
                {
                    Id = 3, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = now.AddDays(-1), LastVersion = 1, Student = student
                }
            );

            await context.SaveChangesAsync();
            _repository = new PollInstanceRepository(context);
            var result = await _repository.GetByCohortIdAndLastDays(1, 10, [100], 7, true, "poll-Uuid", null, null);

            Assert.Equal(1, result.Count);
            Assert.Single(result.Items);
        }

        [Fact]
        public async Task GetByCohortIdAndLastDays_WithZeroDays_UsesNonLastVersionBranchAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1", Name = "Student", Email = "student@test.com"};

            context.Polls.Add(new PollEntity { Uuid = "poll-Uuid", LastVersion = 2 });

            context.StudentCohorts.Add(new StudentCohortJoin { StudentId = 1, CohortId = 100 });

            context.PollInstances.AddRange(
                new PollInstanceEntity
                {
                    Id = 1, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 1, Student = student
                },
                new PollInstanceEntity
                {
                    Id = 2, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow, LastVersion = 2, Student = student
                }
            );

            await context.SaveChangesAsync();

            _repository = new PollInstanceRepository(context);

            var result = await _repository.GetByCohortIdAndLastDays(1, 10, [100], 0, true, "poll-Uuid", null, null);

            Assert.Equal(0, result.Count);
        }

        [Fact]
        public async Task GetByUuidAsync_Returns_Matching_InstanceAsync()
        {
            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "match-Uuid" },
                new PollInstanceEntity { Id = 2, Uuid = "other-Uuid" },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetByUuidAsync("match-Uuid");

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetByUuidAsync_Returns_Null_When_Not_FoundAsync()
        {
            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "other-Uuid" },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetByUuidAsync("missing-Uuid");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUuidAndStudentIdAsync_TwoArgs_Returns_Matching_InstanceAsync()
        {
            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 5 },
                new PollInstanceEntity { Id = 2, Uuid = "poll-Uuid", StudentId = 6 },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetByUuidAndStudentIdAsync("poll-Uuid", 5);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetByUuidAndStudentIdAsync_ThreeArgs_Filters_By_Evaluation_TooAsync()
        {
            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 5, EvaluationId = 10 },
                new PollInstanceEntity { Id = 2, Uuid = "poll-Uuid", StudentId = 5, EvaluationId = 20 },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetByUuidAndStudentIdAsync("poll-Uuid", 5, 20);

            Assert.NotNull(result);
            Assert.Equal(2, result!.Id);
        }

        [Fact]
        public async Task GetImportedStudentsEmailsByPollName_Returns_Distinct_EmailsAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", Student = student },
                new PollInstanceEntity { Id = 2, Uuid = "poll-Uuid", Student = student },
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", Name = "My Poll" }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetImportedStudentsEmailsByPollName("My Poll");

            Assert.Single(result);
            Assert.Equal("student1@test.com", result.First());
        }

        [Theory]
        [InlineData(5, 10, true)]
        [InlineData(999, 10, false)]
        public async Task ExistsForStudentAndEvaluationAsync_Returns_ExpectedAsync(int StudentId, int EvaluationId, bool Expected)
        {
            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 5, EvaluationId = 10 },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.ExistsForStudentAndEvaluationAsync(StudentId, "poll-Uuid", EvaluationId);

            Assert.Equal(Expected, result);
        }

        [Fact]
        public async Task CountByDateRangeAsync_Counts_Instances_Within_RangeAsync()
        {
            var now = DateTime.UtcNow;

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, FinishedAt = now },
                new PollInstanceEntity { Id = 2, FinishedAt = now.AddDays(-50) },
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.CountByDateRangeAsync(now.AddDays(-1), now.AddDays(1));

            Assert.Equal(1, result);
        }

        [Fact]
        public void ComputeAnswersHash_Is_Order_Independent()
        {
            _repository = new PollInstanceRepository(_mockContext.Object);

            var pollA = BuildPollDTO(("Q1", "Yes"), ("Q2", "No"));
            var pollB = BuildPollDTO(("Q2", "No"), ("Q1", "Yes"));

            var hashA = _repository.ComputeAnswersHash(pollA);
            var hashB = _repository.ComputeAnswersHash(pollB);

            Assert.Equal(hashA, hashB);
        }

        [Fact]
        public void ComputeAnswersHash_Differs_For_Different_Answers()
        {
            _repository = new PollInstanceRepository(_mockContext.Object);

            var pollA = BuildPollDTO(("Q1", "Yes"));
            var pollB = BuildPollDTO(("Q1", "No"));

            var hashA = _repository.ComputeAnswersHash(pollA);
            var hashB = _repository.ComputeAnswersHash(pollB);

            Assert.NotEqual(hashA, hashB);
        }

        private static Eras.Application.Dtos.PollDTO BuildPollDTO(params (string Question, string Answer)[] Answers)
        {
            return new Eras.Application.Dtos.PollDTO
            {
                Components =
                [
                    new Eras.Application.DTOs.ComponentDTO
                    {
                        Variables =
                        [
                            .. Answers.Select(A => new Eras.Application.DTOs.VariableDTO
                            {
                                Answer = new Eras.Application.DTOs.AnswerDTO
                                {
                                    Answer = A.Answer
                                }
                            })
                        ]
                    }
                ]
            };
        }

        [Fact]
        public async Task UpdateAsync_Persists_Changes_To_Existing_EntityAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var entity = new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, LastVersion = 1, FinishedAt = DateTime.UtcNow, Student = student };
            context.PollInstances.Add(entity);
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);

            var domainInstance = new Eras.Domain.Entities.PollInstance
            {
                Id = entity.Id,
                Uuid = "updated-Uuid",
                LastVersion = 2,
                FinishedAt = entity.FinishedAt,
                Student = Eras.Infrastructure.Persistence.PostgreSQL.Mappers.StudentMapper.ToDomain(student)
            };

            var result = await _repository.UpdateAsync(domainInstance);

            var updated = await context.PollInstances.FindAsync(entity.Id);
            Assert.NotNull(updated);
            Assert.Equal("updated-Uuid", updated!.Uuid);
            Assert.Equal(2, updated.LastVersion);
        }

        [Fact]
        public async Task UpdateAsync_Returns_Input_When_Entity_Not_FoundAsync()
        {
            using var context = CreateContext();
            _repository = new PollInstanceRepository(context);

            var domainInstance = new Eras.Domain.Entities.PollInstance { Id = 999, Uuid = "missing-Uuid" };

            var result = await _repository.UpdateAsync(domainInstance);

            Assert.Equal("missing-Uuid", result.Uuid);
        }

        [Fact]
        public async Task SetSourceInstanceAsync_Sets_SourcePollInstanceIdAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };
            var entity = new PollInstanceEntity { Uuid = "poll-Uuid", StudentId = 1, FinishedAt = DateTime.UtcNow, Student = student };
            context.PollInstances.Add(entity);
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);

            await _repository.SetSourceInstanceAsync(entity.Id, 999);

            var updated = await context.PollInstances.FindAsync(entity.Id);
            Assert.Equal(999, updated!.SourcePollInstanceId);
        }

        [Fact]
        public async Task SetSourceInstanceAsync_Does_Nothing_When_Instance_Not_FoundAsync()
        {
            using var context = CreateContext();
            _repository = new PollInstanceRepository(context);

            await _repository.SetSourceInstanceAsync(999, 1);
        }

        [Fact]
        public async Task FindMatchingSourceInstanceAsync_Finds_Match_By_Precomputed_HashAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };
            var poll = BuildPollDTO(("Q1", "Yes"), ("Q2", "No"));
            _repository = new PollInstanceRepository(context);
            var hash = _repository.ComputeAnswersHash(poll);

            var existing = new PollInstanceEntity
            {
                Uuid = "poll-Uuid",
                StudentId = 1,
                FinishedAt = DateTime.UtcNow,
                Student = student,
                AnswersHash = hash,
                SourcePollInstanceId = null
            };
            context.PollInstances.Add(existing);
            context.SaveChanges();

            var result = await _repository.FindMatchingSourceInstanceAsync(1, currentPollInstanceId: 999, poll);

            Assert.NotNull(result);
            Assert.Equal(existing.Id, result!.Id);
        }

        [Fact]
        public async Task FindMatchingSourceInstanceAsync_Returns_Null_When_No_MatchAsync()
        {
            using var context = CreateContext();

            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };
            context.PollInstances.Add(new PollInstanceEntity
            {
                Uuid = "poll-Uuid",
                StudentId = 1,
                FinishedAt = DateTime.UtcNow,
                Student = student,
                AnswersHash = "some-other-hash",
                SourcePollInstanceId = null
            });
            context.SaveChanges();

            _repository = new PollInstanceRepository(context);
            var poll = BuildPollDTO(("Q1", "Yes"));

            var result = await _repository.FindMatchingSourceInstanceAsync(1, currentPollInstanceId: 999, poll);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetReportByPollCohortAsync_Aggregates_Averages_By_ComponentAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 1, FinishedAt = DateTime.UtcNow, Student = student }
            }.AsQueryable().BuildMockDbSet();

            var studentCohortData = new List<StudentCohortJoin>
            {
                new StudentCohortJoin { StudentId = 1, CohortId = 100 }
            }.AsQueryable().BuildMockDbSet();

            var studentData = new List<StudentEntity> { student }.AsQueryable().BuildMockDbSet();

            var calcData = new List<ErasCalculationsByPollEntity>
            {
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "How are your grades?",
                    AnswerText = "Good", StudentEmail = "student1@test.com", AnswerRisk = 10,
                    VariableAverageRisk = 10, AnswerPercentage = 50, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                },
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "How are your grades?",
                    AnswerText = "Bad", StudentEmail = "student1@test.com", AnswerRisk = 30,
                    VariableAverageRisk = 20, AnswerPercentage = 50, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                }
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.StudentCohorts).Returns(studentCohortData.Object);
            _mockContext.Setup(C => C.Students).Returns(studentData.Object);
            _mockContext.Setup(C => C.ErasCalculationsByPoll).Returns(calcData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetReportByPollCohortAsync(
                "poll-Uuid", [100], true, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            Assert.NotNull(result);
            Assert.Single(result.Components);
            Assert.Equal("ACADEMIC", result.Components.First().Description);
            Assert.Equal(1, result.PollCount);
        }

        [Fact]
        public async Task GetReportByPollCohortAsync_Excludes_NonApplicable_Answers_From_AveragesAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 1, FinishedAt = DateTime.UtcNow, Student = student }
            }.AsQueryable().BuildMockDbSet();

            var studentCohortData = new List<StudentCohortJoin>
            {
                new StudentCohortJoin { StudentId = 1, CohortId = 100 }
            }.AsQueryable().BuildMockDbSet();

            var studentData = new List<StudentEntity> { student }.AsQueryable().BuildMockDbSet();

            var calcData = new List<ErasCalculationsByPollEntity>
            {
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "Q1",
                    AnswerText = "Ninguno", StudentEmail = "student1@test.com", AnswerRisk = 0,
                    VariableAverageRisk = 0, AnswerPercentage = 100, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                },
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "Q1",
                    AnswerText = "Good", StudentEmail = "student1@test.com", AnswerRisk = 20,
                    VariableAverageRisk = 20, AnswerPercentage = 100, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                }
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.StudentCohorts).Returns(studentCohortData.Object);
            _mockContext.Setup(C => C.Students).Returns(studentData.Object);
            _mockContext.Setup(C => C.ErasCalculationsByPoll).Returns(calcData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetReportByPollCohortAsync(
                "poll-Uuid", [100], true, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));

            Assert.Equal(20, result.Components.First().AverageRisk);
        }

        [Fact]
        public async Task GetReportByPollCohortAsync_Excludes_NonApplicable_Answers_WithoutLastVersionAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "poll-Uuid", StudentId = 1, FinishedAt = DateTime.UtcNow, Student = student }
            }.AsQueryable().BuildMockDbSet();

            var studentCohortData = new List<StudentCohortJoin>
            {
                new StudentCohortJoin { StudentId = 1, CohortId = 100 }
            }.AsQueryable().BuildMockDbSet();

            var studentData = new List<StudentEntity> { student }.AsQueryable().BuildMockDbSet();

            var calcData = new List<ErasCalculationsByPollEntity>
            {
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "Q1",
                    AnswerText = "Ninguno", StudentEmail = "student1@test.com", AnswerRisk = 0,
                    VariableAverageRisk = 0, AnswerPercentage = 100, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                },
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentName = "Academic", Question = "Q1",
                    AnswerText = "Good", StudentEmail = "student1@test.com", AnswerRisk = 20,
                    VariableAverageRisk = 20, AnswerPercentage = 100, PollInstanceId = 1,
                    StudentName = "Student One", CohortName = "Cohort A",
                    PollVersion = 1
                }
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.StudentCohorts).Returns(studentCohortData.Object);
            _mockContext.Setup(C => C.Students).Returns(studentData.Object);
            _mockContext.Setup(C => C.ErasCalculationsByPoll).Returns(calcData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetReportByPollCohortAsync(
                "poll-Uuid", [100], false, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1));
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCountReportByVariablesAsync_Groups_Counts_By_Answer_RiskAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity
                {
                    Id = 1, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow,
                    Student = student, Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "test" }
                }
            }.AsQueryable().BuildMockDbSet();

            var calcData = new List<ErasCalculationsByPollEntity>
            {
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentId = 1, ComponentName = "Academic", Question = "Q1", Position = 1,
                    AnswerText = "Good", StudentEmail = "student1@test.com", StudentName = "Student One",
                    AnswerRisk = 10, VariableAverageRisk = 10, PollInstanceId = 1,
                    CohortId = 100, CohortName = "Cohort A", PollVariableId = 5
                },
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentId = 1, ComponentName = "Academic", Question = "Q1", Position = 1,
                    AnswerText = "Bad", StudentEmail = "student1@test.com", StudentName = "Student One",
                    AnswerRisk = 30, VariableAverageRisk = 20, PollInstanceId = 1,
                    CohortId = 100, CohortName = "Cohort A", PollVariableId = 5
                }
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.ErasCalculationsByPoll).Returns(calcData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetCountReportByVariablesAsync(
                "poll-Uuid", [100], [5], true,
                DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 10);

            Assert.NotNull(result);
            Assert.Single(result.Components);
            var component = result.Components.First();
            Assert.Equal("ACADEMIC", component.Description);
            Assert.Single(component.Questions);
            Assert.Equal(2, component.Questions.First().Answers.Sum(A => A.Count));
        }

        [Fact]
        public async Task GetCountReportByVariablesAsync_Filters_By_EvaluationIdAsync()
        {
            var student = new StudentEntity { Id = 1, Uuid = "student-1-uuid", Name = "Student One", Email = "student1@test.com" };

            var pollInstanceData = new List<PollInstanceEntity>
            {
                new PollInstanceEntity
                {
                    Id = 1, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 10, FinishedAt = DateTime.UtcNow,
                    Student = student, Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "test" }
                },
                new PollInstanceEntity
                {
                    Id = 2, Uuid = "poll-Uuid", StudentId = 1, EvaluationId = 20, FinishedAt = DateTime.UtcNow,
                    Student = student, Audit = new AuditInfo { CreatedAt = DateTime.UtcNow, CreatedBy = "test" }
                }
            }.AsQueryable().BuildMockDbSet();

            var calcData = new List<ErasCalculationsByPollEntity>
            {
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentId = 1, ComponentName = "Academic", Question = "Q1", Position = 1,
                    AnswerText = "Good", StudentEmail = "student1@test.com", StudentName = "Student One",
                    AnswerRisk = 10, VariableAverageRisk = 10, PollInstanceId = 1,
                    CohortId = 100, CohortName = "Cohort A", PollVariableId = 5
                },
                new ErasCalculationsByPollEntity
                {
                    PollUuid = "poll-Uuid", ComponentId = 1, ComponentName = "Academic", Question = "Q1", Position = 1,
                    AnswerText = "Good", StudentEmail = "student1@test.com", StudentName = "Student One",
                    AnswerRisk = 10, VariableAverageRisk = 10, PollInstanceId = 2,
                    CohortId = 100, CohortName = "Cohort A", PollVariableId = 5
                }
            }.AsQueryable().BuildMockDbSet();

            var pollData = new List<PollEntity>
            {
                new PollEntity { Uuid = "poll-Uuid", LastVersion = 1 }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.Polls).Returns(pollData.Object);
            _mockContext.Setup(C => C.PollInstances).Returns(pollInstanceData.Object);
            _mockContext.Setup(C => C.ErasCalculationsByPoll).Returns(calcData.Object);

            _repository = new PollInstanceRepository(_mockContext.Object);

            var result = await _repository.GetCountReportByVariablesAsync(
                "poll-Uuid", [100], [5], true,
                DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 10);

            var totalCount = result.Components.First().Questions.First().Answers.Sum(A => A.Count);
            Assert.Equal(1, totalCount);
        }
    }
}