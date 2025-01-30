using System.Threading.Tasks;
using Eras.Application.DTOs;

namespace Eras.Application.Services
{
    public interface IStudentService
    {
        Task<bool> ImportStudentsAsync(StudentImportDto[] studentsDto);
    }
}
