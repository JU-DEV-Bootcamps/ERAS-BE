using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class AttachmentDraftSessionMapper
    {
        public static AttachmentDraftSession ToDomain(this AttachmentDraftSessionEntity Entity)
        {
            return new AttachmentDraftSession
            {
                Id = Entity.Id,
                CreatedBy = Entity.CreatedBy,
                CreatedAt = Entity.CreatedAt,
            };
        }

        public static AttachmentDraftSessionEntity ToPersistence(this AttachmentDraftSession Model)
        {
            return new AttachmentDraftSessionEntity
            {
                Id = Model.Id,
                CreatedBy = Model.CreatedBy,
                CreatedAt = Model.CreatedAt,
            };
        }
    }
}
