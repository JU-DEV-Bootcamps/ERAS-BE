using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentDetailRepository : BaseRepository<StudentDetail>, IStudentDetailRepository
    {
        public StudentDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}