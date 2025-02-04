using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts
{
    public interface IStudentService
    {
        Task<bool> ImportStudentsAsync(StudentImportDto[] studentsDto);

        Task<Student> CreateStudent(Student student);
    }
}
