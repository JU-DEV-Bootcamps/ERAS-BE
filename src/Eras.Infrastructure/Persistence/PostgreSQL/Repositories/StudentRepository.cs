using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Student?> GetByNameAsync(string name)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(student => student.Name == name);
            
            return student;
        }

        public async Task<Student?> GetByUuidAsync(string uuid)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(student => student.Uuid == uuid);
            
            return student;
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(student => student.Email == email);
            
            return student;
        }
    }
}