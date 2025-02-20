using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
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

        public async Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByComponent(
            string componentName
        )
        {
            var query =
                from s in _context.Students
                join sd in _context.StudentDetails on s.Id equals sd.StudentId
                join pi in _context.PollInstances on s.Id equals pi.StudentId
                join a in _context.Answers on pi.Id equals a.PollInstanceId
                join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                join v in _context.Variables on pv.VariableId equals v.Id
                join c in _context.Components on v.ComponentId equals c.Id
                where
                    c.Name == componentName
                    && a.RiskLevel
                        == _context
                            .Answers.Where(a2 => a2.PollInstanceId == pi.Id)
                            .Max(a2 => a2.RiskLevel)
                orderby a.RiskLevel descending
                select new StudentHeatMapDetailDto
                {
                    StudentId = s.Id,
                    StudentName = s.Name,
                    RiskLevel = a.RiskLevel,
                    ComponentName = c.Name,
                };

            var listDetails = await query.ToListAsync();

            return listDetails;
        }
    }
}
