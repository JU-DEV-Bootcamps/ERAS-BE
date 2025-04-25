using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentCohortRepository(AppDbContext Context)
        : BaseRepository<Student, StudentCohortJoin>(
            Context,
            StudentCohortMapper.ToDomain,
            StudentCohortMapper.ToPersistenceCohort),
        IStudentCohortRepository
    {
        public async Task<Student?> GetByCohortIdAndStudentIdAsync(int CohortId, int StudentId)
        {
            var results = await _context.StudentCohorts.FirstOrDefaultAsync(StudentCohort => StudentCohort.StudentId.Equals(StudentId) && StudentCohort.CohortId.Equals(CohortId));
            return results?.ToDomain();
        }

        public async Task<IEnumerable<Student>?> GetAllStudentsByCohortIdAsync(int CohortId)
        {
            var cohortStudents = await _context.StudentCohorts.Include(Cs => Cs.Student).Where(StudentCohort => StudentCohort.CohortId.Equals(CohortId)).ToListAsync();
            var domainStudents = new List<Student>();
            foreach (var student in cohortStudents)
            {
                domainStudents.Add(student.ToDomain());
            }
            return domainStudents;
        }

        public async Task<List<(Student Student, List<PollInstance> PollInstances)>> GetCohortsSummaryAsync()
        {
            var cohorts = await _context.StudentCohorts.
                Include(Cs => Cs.Cohort).
                Include(Cs => Cs.Student).
                ThenInclude(S => S.PollInstances).
                ThenInclude(P => P.Answers).
                ThenInclude(A => A.PollVariable).
                ToListAsync();
            var cohortsDomain = cohorts.Select(C => (
                Student: C.ToJoinDomain(),
                PollInstances: C.Student.PollInstances.Select(P => P.ToSummaryDomain()).ToList())
            ).ToList();
            return cohortsDomain;
        }
    }
}
