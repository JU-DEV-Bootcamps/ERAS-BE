using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class EvaluationPollMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_EvaluationPollEntity_To_Evaluation()
        {
            var entity = new EvaluationPollJoin
            {
                Id = 1,
                PollId = 2,
                EvaluationId = 3
            };
            var result = EvaluationPollMapper.ToDomain(entity);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.EvaluationPollId);
            Assert.Equal(entity.PollId, result.PollId);
            Assert.Equal(entity.EvaluationId, result.Id);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Evaluation_To_EvaluationPollEntity()
        {
            var entity = new Evaluation
            {
                Id = 1,
                PollId = 2,
                EvaluationPollId = 3
            };
            var result = EvaluationPollMapper.ToPersistence(entity);
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.EvaluationId);
            Assert.Equal(entity.PollId, result.PollId);
            Assert.Equal(entity.EvaluationPollId, result.Id);
        }
    }
}
