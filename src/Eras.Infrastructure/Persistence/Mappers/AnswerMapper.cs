using Eras.Application.Dtos;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.Mappers
{
    public static class AnswerMapper 
    {
        public static AnswersEntity ToAnswerEntity(this Answer answer)
        {
            if (answer == null) throw new ArgumentNullException(nameof(answer));
            return new AnswersEntity
            {
                Id = answer.Id,
                ComponentVariableId = answer.ComponentVariableId,
                Score = (int) answer.RiskLevel,
                Question = answer.Question,
                AnswerText = answer.AnswerText,
                Position = answer.Position,
                CreatedDate = answer.CreatedDate.ToUniversalTime(),
                ModifiedDate = answer.ModifiedDate.ToUniversalTime()
            };
        }
        public static Answer toAnswer(this AnswersEntity answerEntity)
        {
            if (answerEntity == null) throw new ArgumentNullException(nameof(answerEntity));
            return new Answer (
                answerEntity.AnswerText ?? "Answer not found",
                answerEntity.Question ?? "Question not found",
                answerEntity.Position,
                answerEntity.RiskLevel,
                answerEntity.Id,
                answerEntity.ComponentVariableId,
                answerEntity.CreatedDate.DateTime,
                answerEntity.ModifiedDate.DateTime);
        }
    }
}
