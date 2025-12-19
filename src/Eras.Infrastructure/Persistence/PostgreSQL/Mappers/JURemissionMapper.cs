using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class JURemissionMapper
    {
        public static JURemission ToDomain(this JURemissionEntity Entity)
        {
            return new JURemission
            {
                Id = Entity.Id,
                SubmitterUuid = Entity.SubmitterUuid,
                JUServiceId = Entity.JUServiceId,
                JUService = Entity.JUService?.ToDomain(),
                AssignedProfessionalId = Entity.AssignedProfessionalId,
                AssignedProfessional = Entity.AssignedProfessional?.ToDomain(),
                Comment = Entity.Comment,
                Date = Entity.Date,
                Status = Entity.Status,
                StudentIds = Entity.StudentIds,
                Audit = Entity.Audit,
            };
        }

        public static JURemissionEntity ToPersistence(this JURemission Model)
        {
            return new JURemissionEntity
            {
                Id = Model.Id,
                SubmitterUuid = Model.SubmitterUuid,
                JUServiceId = Model.JUServiceId,
                AssignedProfessionalId = Model.AssignedProfessionalId,
                Comment = Model.Comment,
                Date = Model.Date,
                Status = Model.Status,
                StudentIds = Model.StudentIds,
                Audit = Model.Audit,
            };
        }
    }
}
