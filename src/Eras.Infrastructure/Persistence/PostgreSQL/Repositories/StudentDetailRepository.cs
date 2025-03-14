using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
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

        public new async Task<StudentDetail> UpdateAsync(StudentDetail entity)
        {
            var existingEntity = await _context.Set<StudentDetailEntity>().FindAsync(entity.Id);
            if (existingEntity == null)
            {
                throw new Exception("Entity not found");
            }
            _context.Entry(existingEntity).CurrentValues.SetValues(StudentDetailMapper.ToPersistence(entity));
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}