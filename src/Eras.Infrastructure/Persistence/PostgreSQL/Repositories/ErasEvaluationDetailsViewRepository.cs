using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.Response.Controllers.EvaluationDetailsController;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
    string PollUuid, List<string> ComponentNames, List<int> CohortIds,
    List<int>? VariableIds, List<decimal>? RiskLevels,
    int Page, int PageSize, DateTime startDate, DateTime endDate)
    {
        var query = BuildStudentsByFiltersQuery(
        PollUuid, ComponentNames, CohortIds, VariableIds, startDate, endDate);

        var entities = await query
            .OrderBy(v => v.StudentName)
            .Distinct()
            .ToListAsync();

        var filtered = ApplyRiskFilter(entities, RiskLevels).ToList();

        var paged = filtered
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        return paged.Select(ErasEvaluationDetailsViewMapper.ToDomain);
    }

    public async Task<int> CountStudentsByFilters(
        string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevels, DateTime startDate, DateTime endDate)
    {
        var query = BuildStudentsByFiltersQuery(PollUuid, ComponentNames, CohortIds, VariableIds, startDate, endDate);
        var entities = await query.ToListAsync();
        return ApplyRiskFilter(entities, RiskLevels)
            .Select(v => v.StudentId)
            .Distinct()
            .Count();
    }

    private IQueryable<ErasEvaluationDetailsViewEntity> BuildStudentsByFiltersQuery(
        string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, DateTime startDate, DateTime endDate)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>()
            .AsNoTracking()
            .Where(v => v.PollUuid == PollUuid)
            .Where(v => CohortIds.Contains(v.CohortId))
            .Where(v => ComponentNames.Contains(v.ComponentName))
            .Where(v => v.FinishedAt >= startDate && v.FinishedAt <= endDate); 

        if (VariableIds != null && VariableIds.Any())
            query = query.Where(v => VariableIds.Contains(v.VariableId));

        return query;
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