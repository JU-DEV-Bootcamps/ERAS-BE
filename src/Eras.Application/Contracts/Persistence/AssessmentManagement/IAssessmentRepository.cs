using Eras.Domain.Entities.AssessmentManagement;


namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IAssessmentRepository : IBaseRepository<Assessment>
{
    Task<IEnumerable<Assessment>> GetAllAsync();
    
    Task<IEnumerable<Assessment>> GetByStudentIdAsync(int studentId);

    Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status);

    Task<Assessment?> GetByIdWithInterventionsAsync(int id);
    Task DeleteAssessmentAsync(int assessmentId);


    Task<Intervention> AddInterventionAsync(int assessmentId, Intervention intervention); 

    Task<IReadOnlyCollection<Intervention>> ReplaceInterventionsAsync(int assessmentId, IReadOnlyCollection<Intervention> interventions);

    Task DeleteInterventionAsync(int assessmentId, int interventionId);

    Task AddAttachmentsAsync(int interventionId, IReadOnlyCollection<string> paths);
}
