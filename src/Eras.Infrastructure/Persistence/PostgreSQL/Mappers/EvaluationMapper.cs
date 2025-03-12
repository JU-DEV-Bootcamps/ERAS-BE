using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class EvaluationMapper
    {
        public static Evaluation ToDomain(this EvaluationEntity entity)
        {
            return new Evaluation
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = entity.Status,
                PollName = entity.PollName,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Audit = entity.Audit,
            };
        }

        public static EvaluationEntity ToPersistence(this Evaluation model)
        {
            return new EvaluationEntity
            {
                Id = model.Id,
                Name = model.Name,
                PollName = model.PollName,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Audit = model.Audit,
            };
        }
    }
}
