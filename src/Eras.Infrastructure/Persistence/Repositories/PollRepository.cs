using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class PollRepository : IPollRepository<Poll>
    {
        private readonly AppDbContext _context;

        public PollRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Poll> GetTaskByName(string name)
        {
            var tasksByName = await _context.Polls.Where(p => p.Name.Equals(name)).ToListAsync();
            if (tasksByName.Count > 0) return tasksByName[0].ToPoll();
            return null;
        }

        public async Task<Poll> GetPollById(int id)
        {
            var tasksById = await _context.Polls.Where(p => p.Id.Equals(id)).ToListAsync();
            if (tasksById.Count > 0) return tasksById[0].ToPoll();
            return null;
        }

        public async Task<Poll> Add(Poll poll)
        {
            var existingPoll =  await GetTaskByName(poll.PollName);
            if (existingPoll != null) return existingPoll;

            var polltEntity = poll.ToPollEntity();
            _context.Polls.Add(polltEntity);
            await _context.SaveChangesAsync();
            return polltEntity.ToPoll(); 
        }
    }
}
