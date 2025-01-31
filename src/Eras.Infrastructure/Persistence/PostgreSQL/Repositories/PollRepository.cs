using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class PollRepository : IRepository<Poll>, IPollRepository
    {
        private readonly AppDbContext _context;

        public PollRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Poll> Add(Poll entity)
        {
            await _context.Polls.AddAsync(entity);
            
            return entity;
        }

        public Task<Poll> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Poll> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Poll> GetTaskByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Poll> GetPaged(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Poll> Update(Poll entity)
        {
            throw new NotImplementedException();
        }
    }
}