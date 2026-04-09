using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.CohortsController;
using Eras.Application.Utils;
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

        public async Task<CohortSummaryResponse> GetCohortsSummaryAsync(
            Pagination Pagination,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var cohortStudents = _context.StudentCohorts
                .Include(Cs => Cs.Cohort)
                .Include(Cs => Cs.Student)
                .ThenInclude(S => S.PollInstances)
                .ThenInclude(P => P.Answers)
                .ThenInclude(A => A.PollVariable);

            int StudentCount = await cohortStudents.CountAsync();

            var cohorts = await cohortStudents.ToListAsync();

            var cohortsDomain = cohorts.Select(C => new CohortStudentPollsSummary
            {
                Student = C.ToJoinDomain(),
                PollInstances = C.Student.PollInstances
                    .Where(P => (!startDate.HasValue || P.FinishedAt >= startDate) &&
                                (!endDate.HasValue   || P.FinishedAt <= endDate))
                    .Select(P => P.ToSummaryDomain())
                    .ToList()
            })
            .Where(C => C.PollInstances.Any())
            .Skip((Pagination.Page - 1) * Pagination.PageSize)
            .Take(Pagination.PageSize);

            int CohortCount = cohorts.Distinct().Count();

            IEnumerable<StudentSummary> Summary = cohortsDomain.Select(S => new StudentSummary()
            {
                StudentUuid = S.Student.Uuid,
                StudentName = S.Student.Name,
                CohortId = S.Student.Cohort?.Id,
                CohortName = S.Student.Cohort?.Name,
                PollinstancesAverage = S.PollInstances.Any()
                    ? S.PollInstances.Average(P => P.Answers.Average(A => A.RiskLevel))
                    : 0,
                PollinstancesCount = S.PollInstances.Count,
            }).ToList();

            return new CohortSummaryResponse
            {
                CohortCount = CohortCount,
                StudentCount = StudentCount,
                Summary = Summary
            };
        }
    }
}