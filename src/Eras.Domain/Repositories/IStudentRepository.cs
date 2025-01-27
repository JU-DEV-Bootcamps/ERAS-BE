using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IStudentRepository<T>
    {
        Task<T> Add(T student);
        Task<T> GetStudentByEmail(string email);
    }
}