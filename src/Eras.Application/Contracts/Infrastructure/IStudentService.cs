using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IStudentService
    {
        Task<int> ImportStudentsAsync(StudentImportDto[] StudentsDto);

        Task<Student?> CreateStudent(Student Student);
    }
}
