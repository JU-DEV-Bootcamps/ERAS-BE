using System.Diagnostics.CodeAnalysis;

using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class JUServiceRepository : BaseRepository<JUService, JUServiceEntity>, IJUServiceRepository
    {
        public JUServiceRepository(AppDbContext Context)
            : base(Context, JUServiceMapper.ToDomain, JUServiceMapper.ToPersistence) { }

        public new async Task<JUService?> GetByIdAsync(int Id)
        {
            var juService = await _context.JUServices
                .FirstOrDefaultAsync(JUService => JUService.Id == Id);

            return juService?.ToDomain();
        }
        public new async Task<JUService> UpdateAsync(JUService Entity)
        {

            var existingEntity = await _context.Set<JUServiceEntity>().FindAsync(Entity.Id);

            if (existingEntity != null)
            {
                var updatedEntity = JUServiceMapper.ToPersistence(Entity);
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
            }

            return Entity;
        }
    }
}
