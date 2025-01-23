using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.DTOs;
using System;

namespace Eras.Infrastructure.Persistence.Mappers
{
    public static class StudentMapper
    {
        public static StudentDTO ToStudentDTO(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return new StudentDTO(
                student.Id,
                student.CreatedDate,
                student.ModifiedDate,
                student.Name,
                student.Email,
                student.Uuid
            );
        }

        public static Student ToStudent(StudentDTO studentDTO)
        {
            if (studentDTO == null)
                throw new ArgumentNullException(nameof(studentDTO));

            return new Student
            {
                Id = studentDTO.Id,
                CreatedDate = studentDTO.CreatedDate,
                ModifiedDate = studentDTO.ModifiedDate,
                Name = studentDTO.Name,
                Email = studentDTO.Email,
                Uuid = studentDTO.Uuid
            };
        }
    }
}