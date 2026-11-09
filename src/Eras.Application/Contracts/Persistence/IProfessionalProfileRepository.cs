using Eras.Domain.Entities.UserManagement;

namespace Eras.Application.Contracts.Persistence;

public interface IProfessionalProfileRepository
{
    Task<ProfessionalProfile?> GetByErasUserIdAsync(int ErasUserId);

    /// <summary>
    /// Inserts a new profile, or replaces the saved profile with the same Id, courses included.
    /// </summary>
    Task<ProfessionalProfile> SaveAsync(ProfessionalProfile Profile);
}
