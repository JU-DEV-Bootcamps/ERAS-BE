using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs.CL;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using Microsoft.EntityFrameworkCore;

using Evaluation = Eras.Domain.Entities.Evaluation;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class EvaluationRepository : BaseRepository<Evaluation, EvaluationEntity>, IEvaluationRepository
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

        public async new Task<IEnumerable<Evaluation>> GetPagedAsync(int page, int pageSize)
        {
            var persistenceEntity = await _context.Evaluations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(e => e.EvaluationPolls)
                .ThenInclude(ep => ep.Poll)
                .ToListAsync();

            var entities = persistenceEntity.Select(entity =>
            {
                var ev = entity.ToDomain();
                ev.Polls = [.. entity.EvaluationPolls.Select(ep => ep.Poll.ToDomain())];
                return ev;
            });
            return entities;
        }

        public async new Task<List<Evaluation>> GetAllAsync()
        {
            var persistenceEntities = await _context.Evaluations.ToListAsync();
            var evaluationPolls = await _context.EvaluationPolls.Include(ep => ep.Poll).ToListAsync();
            var pollInstances = await _context.PollInstances.Include(pi => pi.Answers).ToListAsync();
            return [.. persistenceEntities.Select(entity => new Evaluation
            {
                Id = entity.Id,
                Name = entity.Name,
                Status = entity.CurrentStatus,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Audit = entity.Audit,
                Polls = [.. evaluationPolls.Where(ep => ep.EvaluationId == entity.Id).Select(ep => PollMapper.ToDomain(ep.Poll))],
                PollInstances = [.. pollInstances.Where(pi => evaluationPolls.Any( ep => ep.Poll.Uuid == pi.Uuid && ep.EvaluationId == entity.Id)).Select(pi => PollInstanceMapper.ToDomain(pi))]
            })];
        }
    }
}
