using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class ProfessionalRepository : BaseRepository<JUProfessional, JUProfessionalEntity>, IProfessionalRepository
    {
        public ProfessionalRepository(AppDbContext Context)
            : base(Context, JUProfessionalMapper.ToDomain, JUProfessionalMapper.ToPersistence) { }


        public new async Task<IEnumerable<JUProfessional>> GetAllAsync()
        {
            var professionals = await _context.Professionals
                .ToListAsync();
            return professionals.Select(Professional => Professional.ToDomain());
        }

        public new async Task<JUProfessional?> GetByIdAsync(int Id)
        {
            var professional = await _context.Professionals
                .FirstOrDefaultAsync(Remission => Remission.Id == Id);

            return professional?.ToDomain();
        }
        public new async Task<JUProfessional> UpdateAsync(JUProfessional Entity)
        {

            var existingEntity = await _context.Set<JUProfessionalEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = JUProfessionalMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
    }
}
