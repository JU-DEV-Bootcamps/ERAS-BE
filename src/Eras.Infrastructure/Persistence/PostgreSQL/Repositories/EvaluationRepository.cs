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
        public EvaluationRepository(AppDbContext Context)
            : base(Context, EvaluationMapper.ToDomain, EvaluationMapper.ToPersistence)
        {
        }
        public async Task<Evaluation?> GetByNameAsync(string Name)
        {
            var evaluation = await _context.Evaluations
                .FirstOrDefaultAsync(Poll => Poll.Name == Name);

            return evaluation?.ToDomain();
        }

        public async Task<Evaluation?> GetByIdForUpdateAsync(int Id)
        {
            var evaluation = await _context.Evaluations
                .AsNoTracking()
                .FirstOrDefaultAsync(E => E.Id == Id);

            return evaluation?.ToDomain();
        }

        public async new Task<IEnumerable<Evaluation>> GetPagedAsync(int Page, int PageSize)
        {
            var persistenceEntity = await _context.Evaluations
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .Include(E => E.EvaluationPolls)
                .ThenInclude(Ep => Ep.Poll)
                .ToListAsync();

            var entities = persistenceEntity.Select(Entity =>
            {
                var ev = Entity.ToDomain();
                ev.Polls = [.. Entity.EvaluationPolls.Select(Ep => Ep.Poll.ToDomain())];
                return ev;
            });
            return entities;
        }

        public async new Task<List<Evaluation>> GetAllAsync()
        {
            var persistenceEntities = await _context.Evaluations.ToListAsync();
            var evaluationPolls = await _context.EvaluationPolls.Include(Ep => Ep.Poll).ToListAsync();
            var pollInstances = await _context.PollInstances.Include(Pi => Pi.Answers).ToListAsync();
            return [.. persistenceEntities.Select(Entity => new Evaluation
            {
                Id = Entity.Id,
                Name = Entity.Name,
                Status = Entity.CurrentStatus,
                StartDate = Entity.StartDate,
                EndDate = Entity.EndDate,
                Audit = Entity.Audit,
                Polls = [.. evaluationPolls.Where(Ep => Ep.EvaluationId == Entity.Id).Select(Ep => PollMapper.ToDomain(Ep.Poll))],
                PollInstances = [.. pollInstances.Where(Pi => evaluationPolls.Any( Ep => Ep.Poll.Uuid == Pi.Uuid && Ep.EvaluationId == Entity.Id)).Select(Pi => PollInstanceMapper.ToDomain(Pi))]
            })];
        }
    }
}
