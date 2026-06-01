using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
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
}
