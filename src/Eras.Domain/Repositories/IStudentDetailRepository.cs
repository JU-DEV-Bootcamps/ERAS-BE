using Eras.Domain.Entities;

namespace Eras.Domain.Repositories
{
    public interface IStudentDetailRepository
    {
        Task<StudentDetail?> GetByIdAsync(int id);
    }
}