using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
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

    public class StudentRepositoryTest
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

        [Fact]
        public async Task BasicGetMethods_ShouldReturnCorrectData()
        {
            var student = new StudentEntity { Id = 10, Name = "Juan", Uuid = "u-1", Email = "j@j.com", IsImported = false };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            Assert.NotNull(await _repository.GetByNameAsync("Juan"));
            Assert.NotNull(await _repository.GetByUuidAsync("u-1"));
            Assert.NotNull(await _repository.GetByEmailAsync("j@j.com"));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateSuccessfully()
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
        public async Task GetStudentHeatMapDetailsByComponent_ShouldHandleComplexJoins()
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
        public async Task GetStudentHeatMapDetailsByCohort_ShouldHandleInvalidId()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repository.GetStudentHeatMapDetailsByCohort("invalid", 5));

            _context.Cohorts.Add(new CohortEntity { Id = 1, Name = "C1", CourseCode = "CODE123" });
            _context.StudentCohorts.Add(new StudentCohortJoin { Id = 1, StudentId = 1, CohortId = 1 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetStudentHeatMapDetailsByCohort("1", 5);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetStudentAverageRiskByCohortsAsync_Coverage()
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
        public async Task GetAverageRiskByStudentIdsAsync_FilteringAndGrouping()
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
    }
}