using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByNameAsync(string name);
        Task<Student?> GetByUuidAsync(string uuid);
        Task<Student?> GetByEmailAsync(string email);
        Task<int> CountAsync();
        Task<(IEnumerable<Student> Students, int TotalCount)> GetAllStudentsByPollUuidAndDaysQuery(int page, int pageSize, string pollUuid, int? days);
    }
}
