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
                Score = answer.Score,
                Question = answer.Question,
                AnswerText = answer.AnswerText,
                Position = answer.Position,
                CreatedDate = answer.CreatedDate,
                ModifiedDate = answer.ModifiedDate
            };
        }
    }
}
