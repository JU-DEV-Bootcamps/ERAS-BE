using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentRepository : BaseRepository<Student, StudentEntity>, IStudentRepository
    {
        public StudentRepository(AppDbContext context)
            : base(context, StudentMapper.ToDomain, StudentMapper.ToPersistence) { }

        public async Task<Student?> GetByNameAsync(string name)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student =>
                student.Name == name
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByUuidAsync(string uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(student =>
                student.Uuid == uuid
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByEmailAsync(string email)
        {
            var student = await _context
                .Students.Include(s => s.StudentDetail)
                .FirstOrDefaultAsync(student => student.Email == email);

            return student?.ToDomain();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Students.CountAsync();
        }
        public async Task<(IEnumerable<Student> Students, int TotalCount)> GetAllStudentsByPollUuidAndDaysQuery(int page, int pageSize, string pollUuid, int days)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            var totalCount = await _context.Students
                .Where(student => student.PollInstances.Any(pollInst => pollInst.Uuid == pollUuid && pollInst.FinishedAt > fromDate))
                .CountAsync();


            var students = await _context.Students
                .Where(student => student.PollInstances.Any(pollInst => pollInst.Uuid == pollUuid && pollInst.FinishedAt > fromDate ))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (students.Select(student => student.ToDomain()), totalCount);
        }
    }
}
