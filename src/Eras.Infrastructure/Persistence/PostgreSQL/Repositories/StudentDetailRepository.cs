using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentDetailRepository : BaseRepository<StudentDetail, StudentDetailEntity>, IStudentDetailRepository
    {
        public StudentDetailRepository(AppDbContext context) 
            : base(context, StudentDetailMapper.ToDomain, StudentDetailMapper.ToPersistence)
        {
        }
    }
}