using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IStudentProfileRepository : IBaseRepository<StudentProfile>
{
    Task<StudentProfile?> GetByStudentCodeAsync(string studentCode);
}
