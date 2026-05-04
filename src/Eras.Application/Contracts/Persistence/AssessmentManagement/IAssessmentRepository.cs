using Eras.Domain.Entities.AssessmentManagement;


namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IAssessmentRepository : IBaseRepository<Assessment>
{
    Task<IEnumerable<Assessment>> GetByStudentIdAsync(int studentId);

    Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status);
}
