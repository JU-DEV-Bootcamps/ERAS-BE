using Eras.Domain.Entities;
using Eras.Domain.Repositories;
using Eras.Infrastructure.Persistence.Mappers;
using Eras.Infrastructure.Persistence.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.Persistence.Repositories
{
    public class AnswerRepository : IAnswerRepository<Answer>
    {
        private readonly AppDbContext _context;

        public AnswerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(Answer answer)
        {
            var answerEntity = answer.ToAnswerEntity();
            _context.Answers.Add(answerEntity);
            await _context.SaveChangesAsync();
        }
    }
}