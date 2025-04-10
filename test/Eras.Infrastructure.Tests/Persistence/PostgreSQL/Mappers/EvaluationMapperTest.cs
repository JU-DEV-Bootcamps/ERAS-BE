using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class EvaluationMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_EvaluationEntity_To_Evaluation()
        {
            var entity = new EvaluationEntity
            {
                Id = 1,
                Name = "Test Evaluation",
                Status = "Active",
                PollName = "Test Poll",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1)
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.PollName, result.PollName);
            Assert.Equal(entity.StartDate, result.StartDate);
            Assert.Equal(entity.EndDate, result.EndDate);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Evaluation_To_EvaluationEntity()
        {
            var model = new Evaluation
            {
                Id = 1,
                Name = "Test Evaluation",
                Status = "Active",
                PollName = "Test Poll",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
            };
            var result = EvaluationMapper.ToPersistence(model);
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.PollName, result.PollName);
            Assert.Equal(model.StartDate, result.StartDate);
            Assert.Equal(model.EndDate, result.EndDate);
        }
    }
}
