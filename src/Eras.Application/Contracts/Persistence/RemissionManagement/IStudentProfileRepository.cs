using Eras.Domain.Entities.RemissionsManagement;


namespace Eras.Application.Contracts.Persistence.RemissionManagement;

public interface IStudentProfileRepository : IBaseRepository<StudentProfile>
{
    Task<StudentProfile?> GetByStudentCodeAsync(string studentCode);
}
