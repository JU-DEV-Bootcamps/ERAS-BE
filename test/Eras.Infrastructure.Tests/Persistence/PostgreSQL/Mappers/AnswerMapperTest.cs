    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class AnswerMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_AnswerEntity_To_Answer()
        {
            var entity = new AnswerEntity
            {
                Id = 1,
                RiskLevel = 1,
                AnswerText = "This is an answer.",
                PollInstanceId = 10,
                PollVariableId = 20
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.RiskLevel, result.RiskLevel);
            Assert.Equal(entity.AnswerText, result.AnswerText);
            Assert.Equal(entity.Audit, result.Audit);
            Assert.Equal(entity.PollInstanceId, result.PollInstanceId);
            Assert.Equal(entity.PollVariableId, result.PollVariableId);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Answer_To_AnswerEntity()
        {
            var model = new Answer
            {
                Id = 1,
                RiskLevel = 1,
                AnswerText = "This is an answer.",
                PollInstanceId = 10,
                PollVariableId = 20
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.RiskLevel, result.RiskLevel);
            Assert.Equal(model.AnswerText, result.AnswerText);
            Assert.Equal(model.Audit, result.Audit);
            Assert.Equal(model.PollInstanceId, result.PollInstanceId);
            Assert.Equal(model.PollVariableId, result.PollVariableId);
        }
    }
}
