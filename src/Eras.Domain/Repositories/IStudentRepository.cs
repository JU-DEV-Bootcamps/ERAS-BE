using Eras.Domain.Entities;
using System.Threading.Tasks;

namespace Eras.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByUuidAsync(string uuid);
        Task<Student> SaveAsync(Student student);
        Task DeleteAsync(string uuid);
    }
}
