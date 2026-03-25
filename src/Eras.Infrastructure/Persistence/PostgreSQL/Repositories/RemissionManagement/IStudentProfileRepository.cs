
using Eras.Domain.Entities.RemissionsManagement;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.RemissionManagement;

public interface IStudentProfileRepository : IBaseRepository<StudentProfile>
{
    Task<StudentProfile?> GetByStudentCodeAsync(string studentCode);
}
