using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class InterventionRepository : BaseRepository<JUIntervention, JUInterventionEntity>, IInterventionRepository
    {
        public InterventionRepository(AppDbContext Context)
            : base(Context, JUInterventionMapper.ToDomain, JUInterventionMapper.ToPersistence) { }


        public new async Task<IEnumerable<JUIntervention>> GetAllAsync()
        {
            var interventions = await _context.Interventions
                .ToListAsync();
            return interventions.Select(Intervention => Intervention.ToDomain());
        }

        public async new Task<IEnumerable<JUIntervention>> GetPagedAsync(int Page, int PageSize)
        {
            var interventions = await _context.Interventions
                .Include(I => I.Student)
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return interventions.Select(I => I.ToDomain());
        }

        public new async Task<JUIntervention?> GetByIdAsync(int Id)
        {
            var intervention = await _context.Interventions
                .FirstOrDefaultAsync(Remission => Remission.Id == Id);

            return intervention?.ToDomain();
        }
        public new async Task<JUIntervention> UpdateAsync(JUIntervention Entity)
        {

            var existingEntity = await _context.Set<JUInterventionEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = JUInterventionMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
    }
}
