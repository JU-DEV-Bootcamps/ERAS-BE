using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.CL;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;
using Evaluation = Eras.Domain.Entities.Evaluation;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class EvaluationRepository: BaseRepository<Evaluation,EvaluationEntity>, IEvaluationRepository
    {
        public EvaluationRepository(AppDbContext context)
            : base(context, EvaluationMapper.ToDomain, EvaluationMapper.ToPersistence)
        {
        }
        public async Task<Evaluation?> GetByNameAsync(string Name)
        {
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(poll => poll.Name == Name);

            return evaluation?.ToDomain();
        }

        public async Task<Evaluation?> GetByIdForUpdateAsync(int id)
        {
            var evaluation = await _context.Evaluations
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            return evaluation?.ToDomain();
        }
    }
}
