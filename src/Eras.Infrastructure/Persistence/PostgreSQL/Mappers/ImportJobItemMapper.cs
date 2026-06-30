using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class ImportJobItemMapper
    {
        public static ImportJobItem ToDomain(this ImportJobItemEntity Entity)
        {
            return new ImportJobItem
            {
                Id = Entity.Id,
                ImportJobId = Entity.ImportJobId,
                StudentEmail = Entity.StudentEmail,
                StudentName = Entity.StudentName,
                Cohort = Entity.Cohort,
                Status = Entity.Status,
                RetryCount = Entity.RetryCount,
                IsAlreadyImported = Entity.IsAlreadyImported,
                ErrorMessage = Entity.ErrorMessage,
                PollPayload = Entity.PollPayload,
                CreatedAtUtc = Entity.CreatedAtUtc,
                UpdatedAtUtc = Entity.UpdatedAtUtc,
            };
        }

        public static ImportJobItemEntity ToPersistence(this ImportJobItem Model)
        {
            return new ImportJobItemEntity
            {
                Id = Model.Id,
                ImportJobId = Model.ImportJobId,
                StudentEmail = Model.StudentEmail,
                StudentName = Model.StudentName,
                Cohort = Model.Cohort,
                Status = Model.Status,
                RetryCount = Model.RetryCount,
                ErrorMessage = Model.ErrorMessage,
                PollPayload = Model.PollPayload,
                CreatedAtUtc = Model.CreatedAtUtc,
                UpdatedAtUtc = Model.UpdatedAtUtc,
            };
        }
    }
}
