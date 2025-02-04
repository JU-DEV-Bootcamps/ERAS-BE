using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<Student?> GetByNameAsync(string name);
        Task<Student?> GetByUuidAsync(string uuid);
        Task<Student?> GetByEmailAsync(string email);
    }
}