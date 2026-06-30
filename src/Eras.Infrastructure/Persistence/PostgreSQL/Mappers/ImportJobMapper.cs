using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ImportJobMapper
    {
        public static ImportJob ToDomain(this ImportJobEntity Entity)
        {
            return new ImportJob
            {
                Id = Entity.Id,
                EvaluationId = Entity.EvaluationId,
                Status = Entity.Status,
                TotalCount = Entity.TotalCount,
                ProcessedCount = Entity.ProcessedCount,
                RetryCount = Entity.RetryCount,
                ErrorMessage = Entity.ErrorMessage,
                PollsPayload = Entity.PollsPayload,
                CreatedAtUtc = Entity.CreatedAtUtc,
                UpdatedAtUtc = Entity.UpdatedAtUtc,
            };
        }

        public static ImportJobEntity ToPersistence(this ImportJob Model)
        {
            return new ImportJobEntity
            {
                Id = Model.Id,
                EvaluationId = Model.EvaluationId,
                Status = Model.Status,
                TotalCount = Model.TotalCount,
                ProcessedCount = Model.ProcessedCount,
                RetryCount = Model.RetryCount,
                ErrorMessage = Model.ErrorMessage,
                PollsPayload = Model.PollsPayload,
                CreatedAtUtc = Model.CreatedAtUtc,
                UpdatedAtUtc = Model.UpdatedAtUtc,
            };
        }
    }
}
