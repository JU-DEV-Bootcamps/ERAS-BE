using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence
{
    public interface IStudentCohortRepository : IBaseRepository<Student>
    {
        Task<Student?> GetByCohortIdAndStudentIdAsync(int cohortId, int studentId);
        Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int cohortId);
        Task<List<(Student Student, List<PollInstance> PollInstances)>> GetCohortsSummaryAsync();
    }
}