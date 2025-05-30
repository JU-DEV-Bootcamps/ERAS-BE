using Eras.Application.DTOs;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure
{
    public interface IStudentService
    {
        int ImportStudentsAsync(StudentImportDto[] StudentsDto);

        Task<Student?> CreateStudent(Student Student);
    }
}
