using Eras.Domain.Entities;
using Eras.Domain.Repositories;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class AnswerRepository : BaseRepository<Answer>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext context)
            : base(context)
        {
        }
    }
}