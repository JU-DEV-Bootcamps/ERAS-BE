using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentMapper
    {
        public static Student ToDomain(this StudentEntity Entity)
        {

            ArgumentNullException.ThrowIfNull(Entity);
            StudentDetail stDetail = Entity.StudentDetail != null? Entity.StudentDetail.ToDomain() : new StudentDetail();
            return new Student
            {
                Id = Entity.Id,
                Name = Entity.Name,
                StudentDetail = stDetail,
                Email = Entity.Email,
                IsImported = Entity.IsImported,
                Uuid = Entity.Uuid,
                Audit = Entity.Audit
            };
        }

        public static StudentEntity ToPersistence(this Student Model)
        {
            ArgumentNullException.ThrowIfNull(Model);
            return new StudentEntity
            {
                Id = Model.Id,
                Name = Model.Name,
                StudentDetail = Model.StudentDetail.ToPersistence(),
                Email = Model.Email,
                IsImported = Model.IsImported,
                Uuid = Model.Uuid,
                Audit = Model.Audit
            };
        }
    }
}
