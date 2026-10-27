using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;


namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public class ErasEvaluationDetailsViewRepository : BaseRepository<Domain.Entities.ErasEvaluationDetailsView, ErasEvaluationDetailsViewEntity>, IErasEvaluationDetailsViewRepository
{
    public ErasEvaluationDetailsViewRepository(AppDbContext Context)
            : base(Context, ErasEvaluationDetailsViewMapper.ToDomain, ErasEvaluationDetailsViewMapper.ToPersistence) { }

    public async Task<List<ErasEvaluationDetailsView>> GetByFiltersAsync(int? PollId, List<int>? ComponentIds, List<int>? CohortIds, List<int>? VariableIds)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>().AsNoTracking();

        if (PollId.HasValue)
        {
            query = query.Where(v => v.PollId == PollId.Value);
        }

        if (CohortIds != null && CohortIds.Any())
        {
            query = query.Where(v => CohortIds.Contains(v.CohortId));
        }

        if (ComponentIds != null && ComponentIds.Any())
        {
            query = query.Where(v => ComponentIds.Contains(v.ComponentId));
        }

        if (VariableIds != null && VariableIds.Any())
        {
            query = query.Where(v => VariableIds.Contains(v.VariableId));
        }

        var entities = await query.ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<StudentsByFiltersResponse>> GetStudentsByEvaluationIdFilters(int EvaluationId, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevels, DateTime startDate, DateTime endDate)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>().AsNoTracking();

        query = query.Where(v => v.EvaluationId == EvaluationId);
        query = query.Where(v => CohortIds.Contains(v.CohortId));
        query = query.Where(v => ComponentNames.Contains(v.ComponentName));
        query = query.Where(v => v.FinishedAt >= startDate && v.FinishedAt <= endDate);

        var entitiesEval = await query.ToListAsync();

        var resultEval = entitiesEval
            .Select(v => new StudentsByFiltersResponse
            {
                Id = v.StudentId,
                Name = v.StudentName,
                Email = v.StudentEmail,
                AnswerId = v.AnswerId,
                AnswerText = v.AnswerText,
                RiskLevel = v.RiskLevel
            })
            .Distinct();

        if (RiskLevels != null && RiskLevels.Any())
            resultEval = resultEval.Where(v => RiskLevels.Contains((decimal)GetRiskGroup((double)v.RiskLevel)));

        return resultEval.ToList();
    }

    public async Task<IEnumerable<ErasEvaluationDetailsView>> GetStudentsByFilters(
    List<string> ComponentNames, List<int> CohortIds,
    List<int>? VariableIds, List<decimal>? RiskLevels,
    int Page, int PageSize, DateTime startDate, DateTime endDate, int? EvaluationId = null)
    {
        var query = await BuildStudentsByFiltersQueryAsync(
            ComponentNames, CohortIds, VariableIds, startDate, endDate, EvaluationId);

        // The view is large (one row per student answer), so first resolve only the students of the
        // requested page from a slim projection and load full rows for those students alone.
        var students = await GetEligibleStudentsAsync(query, RiskLevels);
        var pageIds = students
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Select(S => S.StudentId)
            .ToList();

        if (pageIds.Count == 0)
        {
            return [];
        }

        var entities = await query
            .Where(v => pageIds.Contains(v.StudentId))
            .ToListAsync();

        var firstRowByStudent = ApplyRiskFilter(entities, RiskLevels)
            .GroupBy(v => v.StudentId)
            .ToDictionary(g => g.Key, g => g.First());

        return pageIds
            .Where(firstRowByStudent.ContainsKey)
            .Select(Id => ErasEvaluationDetailsViewMapper.ToDomain(firstRowByStudent[Id]))
            .ToList();
    }

    public async Task<int> CountStudentsByFilters(
        List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevels,
        DateTime startDate, DateTime endDate, int? EvaluationId = null)
    {
        var query = await BuildStudentsByFiltersQueryAsync(
            ComponentNames, CohortIds, VariableIds, startDate, endDate, EvaluationId);
        return (await GetEligibleStudentsAsync(query, RiskLevels)).Count;
    }

    public async Task<int> CountRecentAlerts()
    {
        var itemsRiskStudents = await GetRecentAlertsWithoutPagination();

        return itemsRiskStudents.Count();
    }

    public async Task<IEnumerable<GetStudentsRecentAlertsResponse>> GetRecentAlertsStudentAsync(int Page, int PageSize)
    {
        var averageRiskStudents = await GetRecentAlertsWithoutPagination();

        return averageRiskStudents
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
    private async Task<IEnumerable<GetStudentsRecentAlertsResponse>> GetRecentAlertsWithoutPagination()
    {
        var prevItems = await _context.ErasEvaluationDetailsView
            .Where(e => e.Status == "Completed" || e.Status == "InProgress")
            .Select(e => new
            {
                e.StudentId,
                e.StudentName,
                e.RiskLevel,
                e.ComponentName,
                e.FinishedAt,
                e.Status
            }).ToListAsync();

        return prevItems
            .GroupBy(e => new { e.StudentId, e.ComponentName })
            .Select(e =>
            {
                var averageScore = e.Average(e => (double)e.RiskLevel);
                var first = e.First();
                return new GetStudentsRecentAlertsResponse
                {
                    StudentId = first.StudentId.ToString(),
                    StudentName = first.StudentName,
                    RiskLevel = RiskLevelMapper.ToRiskLevel(averageScore),
                    Category = first.ComponentName,
                    Date = first.FinishedAt,
                    Status = first.Status
                };
            })
            .OrderByDescending(e => e.RiskLevel)
            .ThenByDescending(e => e.Date)
            .ToList();
    }

    private async Task<IQueryable<ErasEvaluationDetailsViewEntity>> BuildStudentsByFiltersQueryAsync(
        List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, DateTime startDate, DateTime endDate, int? EvaluationId = null)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>()
            .AsNoTracking()
            .Where(v => CohortIds.Contains(v.CohortId))
            .Where(v => ComponentNames.Contains(v.ComponentName))
            .Where(v => v.FinishedAt >= startDate && v.FinishedAt <= endDate);

        if (EvaluationId.HasValue)
            query = query.Where(v => v.EvaluationId == EvaluationId.Value);

        // The same question has a different VariableId in each poll of an evaluation, so match by
        // name: expand the selected ids to their names (read from the small variables table, not
        // from the large view) and filter on those.
        if (VariableIds != null && VariableIds.Any())
        {
            var variableNames = await _context.Variables
                .AsNoTracking()
                .Where(v => VariableIds.Contains(v.Id))
                .Select(v => v.Name)
                .Distinct()
                .ToListAsync();
            query = query.Where(v => variableNames.Contains(v.VariableName));
        }

        return query;
    }

    /// <summary>
    /// Students that match the filters, ordered by name. Only (student, name, risk) is read, so the
    /// result is a few rows per student instead of every answer.
    /// </summary>
    private async Task<List<(int StudentId, string StudentName)>> GetEligibleStudentsAsync(
        IQueryable<ErasEvaluationDetailsViewEntity> query, List<decimal>? RiskLevels)
    {
        var slim = await query
            .Select(v => new { v.StudentId, v.StudentName, v.RiskLevel })
            .Distinct()
            .ToListAsync();

        var filtered = RiskLevels == null || !RiskLevels.Any()
            ? slim
            : slim.Where(v => RiskLevels.Contains((decimal)GetRiskGroup((double)v.RiskLevel))).ToList();

        return filtered
            .GroupBy(v => v.StudentId)
            .Select(g => (StudentId: g.Key, StudentName: g.First().StudentName))
            .OrderBy(S => S.StudentName)
            .ThenBy(S => S.StudentId)
            .ToList();
    }

    private static int GetRiskGroup(double risk)
    {
        if (risk < 1) return 0;
        if (risk < 1.5) return 1;
        if (risk < 2.5) return 2;
        if (risk < 3.5) return 3;
        if (risk < 4.5) return 4;
        return 5;
    }

    private IEnumerable<ErasEvaluationDetailsViewEntity> ApplyRiskFilter(
    IEnumerable<ErasEvaluationDetailsViewEntity> entities, List<decimal>? RiskLevels)
    {
        if (RiskLevels == null || !RiskLevels.Any()) return entities;
        return entities.Where(v => RiskLevels.Contains((decimal)GetRiskGroup((double)v.RiskLevel)));
    }
}