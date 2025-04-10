using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentMapper
    {
        public static Student ToDomain(this StudentEntity entity)
        {

            ArgumentNullException.ThrowIfNull(entity);
            StudentDetail stDetail = entity.StudentDetail != null ? entity.StudentDetail.ToDomain() : new StudentDetail();
            return new Student
            {
                Id = entity.Id,
                Name = entity.Name,
                StudentDetail = stDetail,
                Email = entity.Email,
                Uuid = entity.Uuid,
                Audit = entity.Audit
            };
        }

        public static StudentEntity ToPersistence(this Student model)
        {
            ArgumentNullException.ThrowIfNull(model);
            return new StudentEntity
            {
                Id = model.Id,
                Name = model.Name,
                StudentDetail = model.StudentDetail.ToPersistence(),
                Email = model.Email,
                Uuid = model.Uuid,
                Audit = model.Audit
            };
        }
    }
}
