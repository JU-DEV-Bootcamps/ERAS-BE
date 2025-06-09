using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class PollVariableMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_PollVariableJoin_To_Variable()
        {
            var pollVariableJoin = new PollVariableJoin
            {
                Id = 1,
                VariableId = 2,
                PollId = 3,
                Variable = new VariableEntity
                {
                    ComponentId = 4
                },
            };
            var result = pollVariableJoin.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(pollVariableJoin.VariableId, result.Id);
            Assert.Equal(pollVariableJoin.PollId, result.IdPoll);
            Assert.Equal(pollVariableJoin.Variable.ComponentId, result.IdComponent);
            Assert.Equal(pollVariableJoin.Id, result.PollVariableId);
        }

        [Fact]
        public void ToPersistenceVariable_Should_Convert_Variable_To_PollVariableJoin()
        {
            var variable = new Variable
            {
                Id = 2,
                IdPoll = 3
            };
            var result = variable.ToPersistenceVariable();
            Assert.NotNull(result);
            Assert.Equal(variable.Id, result.VariableId);
            Assert.Equal(variable.IdPoll, result.PollId);
        }
    }
}
