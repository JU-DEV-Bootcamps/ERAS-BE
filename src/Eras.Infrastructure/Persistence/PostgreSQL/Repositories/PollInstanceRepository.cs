using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public class PollInstanceRepository(AppDbContext Context) : BaseRepository<PollInstance, PollInstanceEntity>(Context, PollInstanceMapper.ToDomain, PollInstanceMapper.ToPersistence), IPollInstanceRepository
{
    public async Task<PollInstance?> GetByUuidAsync(string Uuid)
    {
        PollInstanceEntity? pollInstance = await _context.PollInstances
            .FirstOrDefaultAsync(PollInstance => PollInstance.Uuid == Uuid);

        return pollInstance?.ToDomain();
    }

    public async Task<PollInstance?> GetByUuidAndStudentIdAsync(string Uuid, int StudentId)
    {
        PollInstanceEntity? results = await _context.PollInstances.FirstOrDefaultAsync(Poll => Poll.Uuid.Equals(Uuid) && Poll.StudentId.Equals(StudentId));
        return results?.ToDomain();
    }


    public async Task<IEnumerable<PollInstance>> GetByLastDays(int Days)
    {
        DateTime dateLimit = DateTime.UtcNow.AddDays(-Days);
        List<PollInstanceEntity> pollInstanceCounts = await _context.PollInstances
        .Include(PollI => PollI.Student)
        .Where(PollI => PollI.FinishedAt >= dateLimit)
        .ToListAsync();

        return pollInstanceCounts.Select(PollInstanceMapper.ToDomain);
    }

    public async Task<IEnumerable<PollInstance>> GetByCohortIdAndLastDays(int? CohortId, int? Days)
    {
        IQueryable<PollInstanceEntity> query = _context.PollInstances.Include(PollI => PollI.Student);

        if (CohortId.HasValue && CohortId != 0)
        {
            query = query
                .Join(_context.StudentCohorts,
                    PollI => PollI.StudentId,
                    StudCohort => StudCohort.StudentId,
                    (PollI, StudCoh) => new { pollInstance= PollI, studentCohort=StudCoh })
                .Where(Joined => Joined.studentCohort.CohortId == CohortId.Value)
                .Select(Joined => Joined.pollInstance);
        }

        if (Days.HasValue && Days != 0)
        {
            DateTime dateLimit = DateTime.UtcNow.AddDays(-Days.Value);
            query = query.Where(PollI => PollI.FinishedAt >= dateLimit);
        }

        List<PollInstanceEntity> pollInstances = await query.Distinct().ToListAsync();
        return [.. pollInstances.Select(PollInstanceMapper.ToDomain)];
    }
    public async Task<PollInstance?> GetAllByPollUuidAsync(Guid PollUuid)
    {
        var pollInstances = _context.PollInstances
            //Get Student name; Get
            .Include(PollI => PollI.Student)
            .ThenInclude(Stud => Stud.StudentCohorts)
            //Get Cohort name; Get Cohort Id
            .ThenInclude(StudCoh => StudCoh.Cohort)
            //Get Risk level; Get Answer text;
            .Include(PollI => PollI.Answers)
            .ThenInclude(Ans => Ans.PollVariable)
            //Get Question text; Component Id
            .ThenInclude(PollV => PollV.Variable)
            //Get Component Name
            .ThenInclude(Var => Var.Component)
            .Where(PollI => PollI.Uuid == PollUuid.ToString()).ToList();

        return pollInstances.Select(PI => PI.ToDomain()).FirstOrDefault();
    }
}
