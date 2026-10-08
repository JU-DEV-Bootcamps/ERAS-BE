using Eras.Domain.Entities.AssessmentManagement;


namespace Eras.Application.Contracts.Persistence.AssessmentManagement;

public interface IAssessmentRepository : IBaseRepository<Assessment>
{
    new Task<IEnumerable<Assessment>> GetAllAsync();
    
    Task<IEnumerable<Assessment>> GetByStudentIdAsync(int studentId);

    Task<IEnumerable<Assessment>> GetByStatusAsync(AssessmentStatus status);

    Task<Assessment?> GetByIdWithInterventionsAsync(int id);
    Task DeleteAssessmentAsync(int assessmentId);


    Task<Intervention> AddInterventionAsync(int assessmentId, Intervention intervention); 

    Task<IReadOnlyCollection<Intervention>> ReplaceInterventionsAsync(int assessmentId, IReadOnlyCollection<Intervention> interventions);

    Task DeleteInterventionAsync(int assessmentId, int interventionId);

    Task AddAttachmentsAsync(int interventionId, IReadOnlyCollection<string> paths, IReadOnlyCollection<string> hashes);
    Task<IReadOnlyCollection<string>> GetAttachmentHashesAsync(int interventionId, CancellationToken cancellationToken);

    Task<Intervention?> GetInterventionByIdAsync(int interventionId);
    Task RemoveAttachmentAsync(int interventionId, string relativePath);

    Task<IEnumerable<Intervention>> GetInterventionsContainingStudentAsync(Assessment entity, IReadOnlyCollection<int> studentToRemoveIds);
    Task<IEnumerable<Assessment>> GetByCreatorAsync(string creatorSub);
    Task<IEnumerable<Assessment>> GetByAssignedProfessionalAsync(string assignedProfessionalSub);

    /// <summary>
    /// Assessments created by or assigned to the user whose status is not <see cref="AssessmentStatus.Finalized"/>.
    /// </summary>
    Task<int> CountActiveAssessmentsForUserAsync(string userSub);

    /// <summary>
    /// Interventions created by the user whose status is not <see cref="InterventionStatus.Finalized"/>.
    /// </summary>
    Task<int> CountActiveInterventionsForUserAsync(string userSub);

    Task<Intervention> UpdateInterventionAsync(int AssessmentId, Intervention Intervention);
}
