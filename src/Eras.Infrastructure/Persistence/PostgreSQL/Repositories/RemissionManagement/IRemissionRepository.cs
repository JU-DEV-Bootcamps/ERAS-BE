
using Eras.Domain.Entities.RemissionsManagement;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories.RemissionManagement;

public interface IRemissionRepository : IBaseRepository<Remission>
{
    Task<IEnumerable<Remission>> GetByStudentIdAsync(Guid studentId);

    Task<IEnumerable<Remission>> GetByStatusAsync(RemissionStatus status);
}
