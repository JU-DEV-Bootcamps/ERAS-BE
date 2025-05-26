using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class EvaluationMapper
    {
        public static Evaluation ToDomain(this EvaluationEntity Entity)
        {
            return new Evaluation
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Status = Entity.CurrentStatus,
                PollName = Entity.PollName,
                Country = Entity.Country,
                StartDate = Entity.StartDate,
                EndDate = Entity.EndDate,
                Audit = Entity.Audit,
            };
        }

        public static EvaluationEntity ToPersistence(this Evaluation Model)
        {
            return new EvaluationEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                PollName = Model.PollName,
                Country = Model.Country,
                Status = Model.Status,
                StartDate = Model.StartDate,
                EndDate = Model.EndDate,
                Audit = Model.Audit,
            };
        }
    }
}
