using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class BaseRepository<TDomain, TPersist> : IBaseRepository<TDomain> 
        where TDomain : class
        where TPersist : class
    {
        protected readonly AppDbContext _context;
        private readonly Func<TPersist, TDomain> _toDomain;
        private readonly Func<TDomain, TPersist> _toPersistence;

        public BaseRepository(
            AppDbContext context, 
            Func<TPersist, TDomain> toDomain, 
            Func<TDomain, TPersist> toPersistence)
        {
            _context = context;
            _toDomain = toDomain;
            _toPersistence = toPersistence;
        }

        public async Task<TDomain> AddAsync(TDomain entity)
        {
            var response = await _context.Set<TPersist>().AddAsync(_toPersistence(entity));
            await _context.SaveChangesAsync();
            
            return _toDomain(response.Entity);
        }

        public async Task DeleteAsync(TDomain entity)
        {
            _context.Set<TPersist>().Remove(_toPersistence(entity));
            await _context.SaveChangesAsync();
        }

        public async Task<TDomain?> GetByIdAsync(int id)
        {
            var persistenceEntity = await _context.Set<TPersist>().FindAsync(id);


            return persistenceEntity != null 
                ? _toDomain(persistenceEntity)
                : null;
        }
        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var persistenceEntities = await _context.Set<TPersist>().ToListAsync();
            return persistenceEntities.Select(entity => _toDomain(entity));
        }


        public async Task<IEnumerable<TDomain>> GetPagedAsync(int page, int pageSize)
        {
            var persistenceEntity = await _context.Set<TPersist>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return persistenceEntity.Select(entity => _toDomain(entity));
        }

        public async Task<TDomain> UpdateAsync(TDomain entity)
        {
            _context.Set<TPersist>().Update(_toPersistence(entity));
            await _context.SaveChangesAsync();
            
            return entity;
        }
    }
}