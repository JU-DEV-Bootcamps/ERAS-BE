﻿using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.HeatMap;
using Eras.Application.DTOs.Student;
using Eras.Application.Utils;
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

        public StudentRepository(AppDbContext Context)
            : base(Context, StudentMapper.ToDomain, StudentMapper.ToPersistence) { }

        public async Task<Student?> GetByNameAsync(string Name)
        {
            var student = await _context.Students.FirstOrDefaultAsync(Student =>
                Student.Name == Name
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByUuidAsync(string Uuid)
        {
            var student = await _context.Students.FirstOrDefaultAsync(Student =>
                Student.Uuid == Uuid
            );

            return student?.ToDomain();
        }

        public async Task<Student?> GetByEmailAsync(string Email)
        {
            var student = await _context
                .Students.Include(S => S.StudentDetail)
                .FirstOrDefaultAsync(Student => Student.Email == Email);

            return student?.ToDomain();
        }
        public async Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByComponent(
            string ComponentName,
            int Limit
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
                    c.Name == ComponentName
                    && a.RiskLevel
                        == _context
                            .Answers.Where(A2 => A2.PollInstanceId == pi.Id)
                            .Max(A2 => A2.RiskLevel)
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
                .OrderByDescending(X => X.RiskLevel)
                .Take(Limit <= 0 ? _defaultLimit : Limit)
                .ToListAsync();

            return listDetails;
        }

        public async Task<List<StudentHeatMapDetailDto>> GetStudentHeatMapDetailsByCohort(
            string CohortId,
            int Limit)
        {
            //comment
            if (!int.TryParse(CohortId, out int cohortInt))
            {
                throw new ArgumentException("Invalid cohort Id format", nameof(CohortId));
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
                                          .Where(A2 => A2.PollInstanceId == pi.Id)
                                          .Max(A2 => A2.RiskLevel)
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
                .OrderByDescending(X => X.RiskLevel)
                .Take(Limit <= 0 ? _defaultLimit : Limit)
                .ToListAsync();

            return listDetails;
        }



        public async Task<(
            IEnumerable<Student> Students,
            int TotalCount
        )> GetAllStudentsByPollUuidAndDaysQuery(
            int Page,
            int PageSize,
            string PollUuid,
            int? Days = null
        )
        {
            DateTime? fromDate = Days > 0 ? DateTime.UtcNow.AddDays(-Days.Value) : null;

            var queryStudents = _context.Students.Where(Student =>
                Student.PollInstances.Any(PollInst => PollInst.Uuid == PollUuid)
            );

            if (fromDate != null)
            {
                queryStudents = queryStudents.Where(Student =>
                    Student.PollInstances.Any(PollInst => PollInst.FinishedAt > fromDate)
                );
            }

            var totalCount = await queryStudents.CountAsync();
            var students = await queryStudents
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return (students.Select(Student => Student.ToDomain()), totalCount);
        }

        public Task<PagedResult<StudentAverageRiskDto>> GetStudentAverageRiskByCohortsAsync(
            Pagination Pagination,
            List<int> CohortIds,
            string PollUuid,
            bool LastVersion
        )
        {
            int pollVersion = _context.Polls
            .Where(A => A.Uuid == PollUuid)
            .Select(A => A.LastVersion)
            .FirstOrDefault();
            IQueryable<StudentAverageRiskDto> query;
            if (LastVersion)
            {
            query = _context.ErasCalculationsByPoll
                .Where(X => CohortIds.Contains(X.CohortId) && X.PollUuid == PollUuid && X.PollVersion == pollVersion)
                .GroupBy(X => X.StudentEmail)
                .Select(G => new StudentAverageRiskDto
                {
                    StudentId = G.First().StudentId,
                    StudentName = G.First().StudentName,
                    Email = G.Key,
                    AvgRiskLevel = G.Average(X => X.AnswerRisk)
                });
            }
            else
            {
                query = _context.ErasCalculationsByPoll
                    .Where(X => CohortIds.Contains(X.CohortId) && X.PollUuid == PollUuid && X.PollVersion != pollVersion)
                    .GroupBy(X => X.StudentEmail)
                    .Select(G => new StudentAverageRiskDto
                    {
                        StudentId = G.First().StudentId,
                        StudentName = G.First().StudentName,
                        Email = G.Key,
                        AvgRiskLevel = G.Average(X => X.AnswerRisk)
                    });
            }

            var count = query.Count();

            var data = query
                .OrderByDescending(X => X.AvgRiskLevel)
                .Skip((Pagination.Page - 1) * Pagination.PageSize)
                .Take(Pagination.PageSize)
                .ToList();

            return Task.FromResult(new PagedResult<StudentAverageRiskDto>(count, data));

        }

        public new async Task<Student> UpdateAsync(Student Entity)
        {

            var existingEntity = await _context.Set<StudentEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = StudentMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
        public async Task<IEnumerable<Student>> GetPagedAsyncWithJoins(int Page, int PageSize)
        {
            var persistenceEntity = await _context.Set<StudentEntity>()
                .OrderBy(Stu => Stu.Name)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Include(E => E.StudentDetail)
                .ToListAsync();

            return persistenceEntity.Select(Entity => StudentMapper.ToDomain(Entity));
        }
    }
}
