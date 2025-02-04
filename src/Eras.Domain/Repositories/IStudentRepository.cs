using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByUuidAsync(string uuid);
        Task<Student> SaveAsync(Student student);
        Task DeleteAsync(string uuid);
    }
}
