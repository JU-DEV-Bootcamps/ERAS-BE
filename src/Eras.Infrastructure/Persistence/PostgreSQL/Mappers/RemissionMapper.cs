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
                JUService = Entity.JUService?.ToDomain() ?? new JUService(),
                AssignedProfessional = Entity.AssignedProfessional?.ToDomain() ?? new JUProfessional(),
                Comment = Entity.Comment,
                Date = Entity.Date,
                Status = Entity.Status,
                Students = Entity.Students.Select(Stu => Stu.ToDomain()).ToList() ?? [],
                Audit = Entity.Audit,
            };
        }

        public static JURemissionEntity ToPersistence(this JURemission Model)
        {
            return new JURemissionEntity
            {
                Id = Model.Id,
                SubmitterUuid = Model.SubmitterUuid,
                JUService = Model.JUService?.ToPersistence() ?? null,
                AssignedProfessionalUuid = Model.AssignedProfessional.Uuid,
                Comment = Model.Comment,
                Date = Model.Date,
                Status = Model.Status,
                Students = Model.Students.Select(Stu => Stu.ToPersistence()).ToList() ?? [],
                Audit = Model.Audit,
            };
        }
    }
}
