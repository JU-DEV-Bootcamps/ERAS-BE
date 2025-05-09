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
internal class PollVersionRepository: BaseRepository<PollVersion,PollVersionEntity>, IPollVersionRepository
{
    public PollVersionRepository(AppDbContext Context)
            : base(Context, PollVersionMapper.ToDomain, PollVersionMapper.ToPersistence) { }

    public async Task<PollVersion?> GetByPollAndVersionAsync(string VersionName, int PollId)
    {
        var query = await _context.PollVersion.
            Where(Pv => Pv.Name == VersionName && Pv.PollId == PollId).FirstOrDefaultAsync();
        if (query == null)
            return null;
        return query.ToDomain();
    }
    public async Task<List<PollVersion>> GetAllByPollAsync(int PollId)
    {
        var query = await _context.PollVersion.Where(Pv => Pv.PollId == PollId)
            .OrderByDescending(Pv => Pv.Date)
            .ToListAsync();
        return query.Select(Pv => Pv.ToDomain()).ToList();
    }
}
