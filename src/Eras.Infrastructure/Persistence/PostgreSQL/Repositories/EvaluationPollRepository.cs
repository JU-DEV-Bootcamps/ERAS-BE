using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    internal class EvaluationPollRepository(AppDbContext context) : BaseRepository<Evaluation, EvaluationPollJoin>(context, EvaluationPollMapper.ToDomain, EvaluationPollMapper.ToPersistence), IEvaluationPollRepository
    {
        public List<Evaluation> GetAllPollsPollInstances()
        {
            var result = _context.EvaluationPolls
                .Include(ep => ep.Evaluation)
                .Include(ep => ep.Poll)
                .Join(_context.PollInstances,
                    ep => ep.Poll.Uuid,
                    pi => pi.Uuid,
                    (ep, pi) => new { ep, pi })
                .Select(x => new Evaluation
                {
                    Name = x.ep.Evaluation.Name,
                    Status = x.ep.Evaluation.Status,
                    StartDate = x.ep.Evaluation.StartDate,
                    EndDate = x.ep.Evaluation.EndDate,
                    Polls = [ x.ep.Poll ],
                    PollInstances = [ x.pi ],
                }).ToList();

            return result;
        }
    }
}
