﻿
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Moq;
using MockQueryable.Moq;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Repositories
{
    public class HeatMapRepositoryTest
    {
        private readonly Mock<AppDbContext> _mockContext;
        private readonly HeatMapRespository _repository;

        public HeatMapRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "HeatMapTest")
                .Options;
            _mockContext = new Mock<AppDbContext>(options);
            _repository = new HeatMapRespository(_mockContext.Object);

            SeedData();
        }

        private void SeedData()
        {
            var pollInstances = new List<PollInstanceEntity>
            {
                new PollInstanceEntity { Id = 1, Uuid = "uuid1", FinishedAt = DateTime.UtcNow.AddDays(-5), StudentId = 1 },
                new PollInstanceEntity { Id = 2, Uuid = "uuid2", FinishedAt = DateTime.UtcNow.AddDays(-10), StudentId = 2 }
            }.AsQueryable().BuildMockDbSet();

            var variables = new List<VariableEntity>
            {
                new VariableEntity { Id = 1, ComponentId = 1 }
            }.AsQueryable().BuildMockDbSet();

            var components = new List<ComponentEntity>
            {
                new ComponentEntity { Id = 1, Name = "Component1" }
            }.AsQueryable().BuildMockDbSet();

            var pollVariableJoins = new List<PollVariableJoin>
            {
                new PollVariableJoin { Id = 1, VariableId = 1 }
            }.AsQueryable().BuildMockDbSet();

            var answers = new List<AnswerEntity>
            {
                new AnswerEntity { Id = 1, PollInstanceId = 1, PollVariableId = 1, AnswerText = "Answer1", RiskLevel = 1 },
                new AnswerEntity { Id = 2, PollInstanceId = 2, PollVariableId = 1, AnswerText = "Answer2", RiskLevel = 2 }
            }.AsQueryable().BuildMockDbSet();

            var studentCohorts = new List<StudentCohortJoin>
            {
                new StudentCohortJoin { Id = 1, StudentId = 1, CohortId = 1 },
                new StudentCohortJoin { Id = 2, StudentId = 2, CohortId = 2 }
            }.AsQueryable().BuildMockDbSet();

            var cohorts = new List<CohortEntity>
            {
                new CohortEntity { Id = 1, Name = "Cohort1", CourseCode = "Course1" },
                new CohortEntity { Id = 2, Name = "Cohort2", CourseCode = "Course2" }
            }.AsQueryable().BuildMockDbSet();

            _mockContext.Setup(C => C.PollInstances).Returns(pollInstances.Object);
            _mockContext.Setup(C => C.Variables).Returns(variables.Object);
            _mockContext.Setup(C => C.Components).Returns(components.Object);
            _mockContext.Setup(C => C.Set<PollVariableJoin>()).Returns(pollVariableJoins.Object);
            _mockContext.Setup(C => C.Answers).Returns(answers.Object);
            _mockContext.Setup(C => C.StudentCohorts).Returns(studentCohorts.Object);
            _mockContext.Setup(C => C.Cohorts).Returns(cohorts.Object);
        }

        [Fact]
        public async Task GetHeatMapDataByCohortAndDaysShouldReturnDataByDaysAsync()
        {
            // Act
            var result = await _repository.GetHeatMapDataByCohortAndDaysAsync(0, 7);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Component1", result.First().ComponentName);
        }

        [Fact]
        public async Task GetHeatMapDataByCohortAndDaysShouldReturnDataByCohortIdAsync()
        {
            // Act
            var result = await _repository.GetHeatMapDataByCohortAndDaysAsync(2, 0);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Component1", result.First().ComponentName);
        }

        [Fact]
        public async Task GetHeatMapDataByCohortAndDaysShouldReturnEmptyWhenNoDataExistsAsync()
        {
            // Act
            var result = await _repository.GetHeatMapDataByCohortAndDaysAsync(2, 7);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetHeatMapDataByCohortAndDaysShouldReturnDataWhenCohortIdAndDaysAreZeroAsync()
        {
            // Act
            var result = await _repository.GetHeatMapDataByCohortAndDaysAsync(0, 0);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
