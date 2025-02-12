using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Mappers
{
    public static class StudentMapper
    {
        public static Student ToDomain(this StudentEntity entity)
        {
            return new Student
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Uuid = entity.Uuid,
                Audit = entity.Audit
            };
        }

        public static StudentEntity ToPersistence(this Student model)
        {
            return new StudentEntity
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Uuid = model.Uuid,
                Audit = model.Audit
            };
        }
    }
}