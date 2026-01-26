using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            query = query.Where(v => v.CohortId.HasValue && CohortIds.Contains(v.CohortId.Value));
        }

        if (ComponentIds != null && ComponentIds.Any())
        {
            query = query.Where(v => v.ComponentId.HasValue && ComponentIds.Contains(v.ComponentId.Value));
        }

        if (VariableIds != null && VariableIds.Any())
        {
            query = query.Where(v => v.VariableId.HasValue && VariableIds.Contains(v.VariableId.Value));
        }

        var entities = await query.ToListAsync();

        return entities.Select(e => e.ToDomain()).ToList();
    }

    public async Task<List<StudentsByFiltersResponse>> GetStudentsByFilters(int? PollId, List<int>? ComponentIds, List<int>? CohortIds, List<int>? VariableIds)
    {
        var query = _context.Set<ErasEvaluationDetailsViewEntity>().AsNoTracking();

        if (PollId.HasValue)
            query = query.Where(v => v.PollId == PollId.Value);

        if (CohortIds?.Any() == true)
            query = query.Where(v => v.CohortId.HasValue && CohortIds.Contains(v.CohortId.Value));

        if (ComponentIds?.Any() == true)
            query = query.Where(v => v.ComponentId.HasValue && ComponentIds.Contains(v.ComponentId.Value));

        if (VariableIds?.Any() == true)
            query = query.Where(v => v.VariableId.HasValue && VariableIds.Contains(v.VariableId.Value));

        return await query
            .Select(v => new StudentsByFiltersResponse
            {
                Id = v.StudentId ?? 0,
                Name = v.StudentName ?? string.Empty,
                Email = v.StudentEmail ?? string.Empty,
                AnswerId = v.AnswerId ?? 0,
                AnswerText = v.AnswerText ?? string.Empty,
                RiskLevel = v.RiskLevel ?? 0
            })
            .ToListAsync();
    }
}
