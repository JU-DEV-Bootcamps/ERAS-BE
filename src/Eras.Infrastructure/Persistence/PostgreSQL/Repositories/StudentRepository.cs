using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
using Eras.Application.DTOs.Student;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class StudentRepository : BaseRepository<Student, StudentEntity>, IStudentRepository
    {
        private const int _defaultLimit = 5;

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
        public async Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByComponent(
            string componentName,
            int limit
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

            var listDetails = await query
                .Distinct()
                .OrderByDescending(x => x.RiskLevel)
                .Take(limit <= 0 ? _defaultLimit : limit)
                .ToListAsync();

            return listDetails;
        }

        public async Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByCohort(
            string cohortId,
            int limit)
        {
            if (!int.TryParse(cohortId, out int cohortInt))
            {
                throw new ArgumentException("Invalid cohort Id format", nameof(cohortId));
            }

            var query =
                from s in _context.Students
                join sc in _context.StudentCohorts on s.Id equals sc.StudentId
                join cohort in _context.Cohorts on sc.CohortId equals cohort.Id
                join sd in _context.StudentDetails on s.Id equals sd.StudentId
                join pi in _context.PollInstances on s.Id equals pi.StudentId
                join a in _context.Answers on pi.Id equals a.PollInstanceId
                where cohort.Id == cohortInt
                      && a.RiskLevel == _context.Answers
                                          .Where(a2 => a2.PollInstanceId == pi.Id)
                                          .Max(a2 => a2.RiskLevel)
                orderby a.RiskLevel descending
                select new StudentHeatMapDetailDto
                {
                    StudentId = s.Id,
                    StudentName = s.Name,
                    RiskLevel = a.RiskLevel,
                    ComponentName = cohort.Name,
                };

            var listDetails = await query
                .Distinct()
                .OrderByDescending(x => x.RiskLevel)
                .Take(limit <= 0 ? _defaultLimit : limit)
                .ToListAsync();

            return listDetails;
        }



        public async Task<(
            IEnumerable<Student> Students,
            int TotalCount
        )> GetAllStudentsByPollUuidAndDaysQuery(
            int page,
            int pageSize,
            string pollUuid,
            int? days = null
        )
        {
            DateTime? fromDate = days > 0 ? DateTime.UtcNow.AddDays(-days.Value) : null;

            var queryStudents = _context.Students.Where(student =>
                student.PollInstances.Any(pollInst => pollInst.Uuid == pollUuid)
            );

            if (fromDate != null)
            {
                queryStudents = queryStudents.Where(student =>
                    student.PollInstances.Any(pollInst => pollInst.FinishedAt > fromDate)
                );
            }

            var totalCount = await queryStudents.CountAsync();
            var students = await queryStudents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (students.Select(student => student.ToDomain()), totalCount);
        }

        public async Task<List<StudentAverageRiskDto>> GetStudentAverageRiskAsync(
            int cohortId,
            int pollId
        )
        {
            var query =
                from s in _context.Students
                join sc in _context.StudentCohorts on s.Id equals sc.StudentId
                join pi in _context.PollInstances on s.Id equals pi.StudentId
                join a in _context.Answers on pi.Id equals a.PollInstanceId
                join pv in _context.PollVariables on a.PollVariableId equals pv.Id
                join c in _context.Cohorts on sc.CohortId equals c.Id
                where c.Id == cohortId && pv.PollId == pollId
                group a by new
                {
                    s.Id,
                    s.Name,
                    s.Email,
                } into g
                select new StudentAverageRiskDto
                {
                    StudentId = g.Key.Id,
                    StudentName = g.Key.Name,
                    Email = g.Key.Email,
                    AvgRiskLevel = g.Average(x => x.RiskLevel),
                };

            return await query.OrderByDescending(x => x.AvgRiskLevel).ToListAsync();
        }

        public new async Task<Student> UpdateAsync(Student entity)
        {
          
            var existingEntity = await _context.Set<StudentEntity>().FindAsync(entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = StudentMapper.ToPersistence(entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return entity;
        }
    }
}
