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
                AssignedProfessionalId = Entity.AssignedProfessionalId,
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
                JUServiceId = Model.JUServiceId,
                AssignedProfessionalId = Model.AssignedProfessionalId,
                Comment = Model.Comment,
                Date = Model.Date,
                Status = Model.Status,
                Students = Model.Students.Select(Stu => Stu.ToPersistence()).ToList() ?? [],
                Audit = Model.Audit,
            };
        }
    }
}
