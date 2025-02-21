using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class AnswerRepository : BaseRepository<Answer, AnswerEntity>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext context)
            : base(context, AnswerMapper.ToDomain, AnswerMapper.ToPersistence)
        {

        }
        public async Task SaveManyAnswersAsync(List<Answer> answers)
        {
            await _context.Answers.AddRangeAsync(answers.Select(ans => ans.ToPersistence()));
            var result =  await _context.SaveChangesAsync();
            // return result.;
        }
    }
}