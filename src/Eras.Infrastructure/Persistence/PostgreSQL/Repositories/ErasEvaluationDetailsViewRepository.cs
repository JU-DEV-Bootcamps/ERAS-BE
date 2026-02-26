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

using Microsoft.AspNetCore.Mvc.RazorPages;
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

    public async Task<List<StudentsByFiltersResponse>> GetStudentsByEvaluationIdFilters(int EvaluationId, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<int>? RiskLevels)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>().AsNoTracking();

        query = query.Where(v => v.EvaluationId == EvaluationId);
        query = query.Where(v => CohortIds.Contains(v.CohortId));
        query = query.Where(v => ComponentNames.Contains(v.ComponentName));

        if (VariableIds != null && VariableIds.Any())
        {
            query = query.Where(v => VariableIds.Contains(v.VariableId));
        }

        if (RiskLevels != null && RiskLevels.Any())
        {
            query = query.Where(v => RiskLevels.Contains((int)v.RiskLevel));
        }

        return await query
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
        {
            resultEval = resultEval.Where(v => RiskLevels.Contains(Math.Floor(v.RiskLevel)));
        }

        return resultEval.ToList();
    }

    public async Task<(IEnumerable<ErasEvaluationDetailsView> Items, int Total)> GetStudentsByFilters(
        string PollUuid, List<string> ComponentNames, List<int> CohortIds, List<int>? VariableIds, List<decimal>? RiskLevels, int Page, int PageSize)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>()
            .AsNoTracking()
            .Where(v => v.PollUuid == PollUuid)
            .Where(v => CohortIds.Contains(v.CohortId))
            .Where(v => ComponentNames.Contains(v.ComponentName));

        if (VariableIds != null && VariableIds.Any())
        {
            query = query.Where(v => VariableIds.Contains(v.VariableId));
        }

        if (RiskLevels != null && RiskLevels.Any())
        {
            result = result.Where(v => RiskLevels.Contains(Math.Floor(v.RiskLevel)));
        }
        
        var entities = await query.ToListAsync();

        int total = await query.Select(v => v.StudentId).CountAsync();

        var persistenceEntities = await query
            .OrderBy(Eval => Eval.StudentName)
            .Skip((Page - 1) * PageSize)
            .Take(PageSize)
            .Distinct()
            .ToListAsync(); 
        var items = persistenceEntities.Select(Eval => ErasEvaluationDetailsViewMapper.ToDomain(Eval)); 
        
        return (items, total);
    }
}
