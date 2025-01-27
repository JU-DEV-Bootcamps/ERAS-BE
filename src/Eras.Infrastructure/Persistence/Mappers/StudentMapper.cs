using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL;

namespace Eras.Infrastructure.Persistence.Mappers
{
    public static class StudentMapper
    {
        public static StudentEntity ToStudentEntity(this Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return new StudentEntity
            {
                Id = student.Id,
                CreatedDate = student.CreatedDate.ToUniversalTime(),
                ModifiedDate = student.ModifiedDate.ToUniversalTime(),
                Name = student.Name,
                Email = student.Email,
                Uuid = student.Uuid
            };
        }
        public static Student ToStudent(this StudentEntity studentEntity)
        {
            if (studentEntity == null)
                throw new ArgumentNullException(nameof(studentEntity));
            return new Student
            {
                Id = studentEntity.Id,
                CreatedDate = studentEntity.CreatedDate.DateTime,
                ModifiedDate = studentEntity.ModifiedDate.DateTime,
                Name = studentEntity.Name,
                Email = studentEntity.Email,
                Uuid = studentEntity.Uuid
            };
        }
        
    }
}