using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentDetailRepository : IBaseRepository<StudentDetail>
    {
        Task<StudentDetail?> GetByStudentId(int studentId);
    }
}
