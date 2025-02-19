using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentDetailRepository : BaseRepository<StudentDetail, StudentDetailEntity>, IStudentDetailRepository
    {
        public StudentDetailRepository(AppDbContext context) 
            : base(context, StudentDetailMapper.ToDomain, StudentDetailMapper.ToPersistence)
        {
        }

        public async Task<StudentDetail?> GetByStudentId(int studentId)
        {
            var studentDetail = await _context.StudentDetails
                .FirstOrDefaultAsync(studentDetail =>  studentDetail.StudentId == studentId);

            return studentDetail?.ToDomain();
        }
    }
}