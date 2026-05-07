using Eras.Domain.Entities.AssessmentManagement;


namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IAssessmentRepository : IBaseRepository<Assessment>
{
    Task<IEnumerable<Assessment>> GetByStudentIdAsync(Guid studentId);

    Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status);

    Task<Assessment?> GetByIdWithInterventionsAsync(Guid id);

    Task<Intervention> AddInterventionAsync(Guid assessmentId, Intervention intervention); 

    Task<IReadOnlyCollection<Intervention>> ReplaceInterventionsAsync(Guid assessmentId, IReadOnlyCollection<Intervention> interventions);

    Task DeleteInterventionAsync(Guid assessmentId, Guid interventionId);
}
