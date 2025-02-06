using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentDetailRepository
    {
        Task<StudentDetail?> GetByIdAsync(int id);
    }
}