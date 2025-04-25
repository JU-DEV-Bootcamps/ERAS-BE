using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class PollInstanceMapper
    {
        public static PollInstance ToDomain(this PollInstanceEntity Entity)
        {
            return new PollInstance
            {
                Id = Entity.Id,
                Uuid = Entity.Uuid,
                Student = Entity.Student?.ToDomain() ?? new Student(),
                Answers = Entity.Answers?.Select(Ans => Ans.ToDomain()).ToList() ?? [],
                Audit = Entity.Audit,
                FinishedAt = Entity.FinishedAt

            };
        }

        public static PollInstance ToSummaryDomain(this PollInstanceEntity Entity)
        {
            return new PollInstance
            {
                Id = Entity.Id,
                Uuid = Entity.Uuid,
                Student = Entity.Student?.ToDomain() ?? new Student(),
                FinishedAt = Entity.FinishedAt,
                Answers = [.. Entity.Answers.Select(A => A.ToDomain())],
            };
        }

        public static PollInstanceEntity ToPersistence(this PollInstance Model)
        {
            return new PollInstanceEntity
            {
                Id = Model.Id,
                Uuid = Model.Uuid,
                StudentId = Model.Student != null ? Model.Student.Id : 0,
                Audit = Model.Audit,
                FinishedAt = Model.FinishedAt
            };
        }
    }
}
