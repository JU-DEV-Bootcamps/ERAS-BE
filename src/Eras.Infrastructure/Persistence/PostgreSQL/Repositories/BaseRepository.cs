using System.Diagnostics.CodeAnalysis;
using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BaseRepository<TDomain, TPersist> : IBaseRepository<TDomain> 
        where TDomain : class
        where TPersist : class
    {
        protected readonly AppDbContext _context;
        private readonly Func<TPersist, TDomain> _toDomain;
        private readonly Func<TDomain, TPersist> _toPersistence;

        public BaseRepository(
            AppDbContext Context,
            Func<TPersist, TDomain> ToDomain,
            Func<TDomain, TPersist> ToPersistence)
        {
            _context = Context;
            _toDomain = ToDomain;
            _toPersistence = ToPersistence;
        }

        public async Task<TDomain> AddAsync(TDomain Entity)
        {
            var response = await _context.Set<TPersist>().AddAsync(_toPersistence(Entity));
            await _context.SaveChangesAsync();
            
            return _toDomain(response.Entity);
        }
        public async Task DeleteAsync(TDomain Entity)
        {
            _context.Set<TPersist>().Remove(_toPersistence(Entity));
            await _context.SaveChangesAsync();
        }

        public async Task<TDomain?> GetByIdAsync(int Id)
        {
            var persistenceEntity = await _context.Set<TPersist>().FindAsync(Id);


            return persistenceEntity != null 
                ? _toDomain(persistenceEntity)
                : null;
        }
        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var persistenceEntities = await _context.Set<TPersist>().ToListAsync();
            return persistenceEntities.Select(Entity => _toDomain(Entity));
        }


        public async Task<IEnumerable<TDomain>> GetPagedAsync(int Page, int PageSize)
        {
            var persistenceEntity = await _context.Set<TPersist>()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            
            return persistenceEntity.Select(Entity => _toDomain(Entity));
        }
        public async Task<int> CountAsync()
        {
            return await _context.Set<TPersist>().CountAsync();
        }

        public async Task<TDomain> UpdateAsync(TDomain Entity)
        {
            _context.Set<TPersist>().Update(_toPersistence(Entity));
            await _context.SaveChangesAsync();
            
            return Entity;
        }
    }
}
