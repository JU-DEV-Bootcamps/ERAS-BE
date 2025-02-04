using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentDetailRepository : BaseRepository<StudentDetail>, IStudentDetailRepository
    {
        public StudentDetailRepository(AppDbContext context) : base(context)
        {
        }
    }
}