using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentPollsRepository : BaseRepository<Poll, PollEntity>, IStudentPollsRepository
    {

        public StudentPollsRepository(AppDbContext Context)
            : base(Context, Mappers.PollMapper.ToDomain, Mappers.PollMapper.ToPersistence)
        {
        }

        public async Task<List<Poll>> GetPollsByStudentIdAsync(int StudentId)
        {
            var pollsAsync = await _context.Polls
                .Where(P => P.PollVariables
                    .Any(Pv => Pv.Answers
                        .Any(A => A.PollInstance.StudentId == StudentId)))
                .Select(P => new
                {
                    P.Id,
                    P.Uuid,
                    P.Name,
                    P.Version
                })
                .Distinct()
                .ToListAsync();
            var polls = pollsAsync.Select(P => new PollEntity
            {
                Id = P.Id,
                Uuid = P.Uuid,
                Name = P.Name,
                Version = P.Version,
                PollVariables = _context.PollVariables
                .Where(Pv => Pv.PollId == P.Id)
                .Select(Pv => new PollVariableJoin
                {
                    Id = Pv.Id,
                    Answers = Pv.Answers
                    .Where(A => A.PollInstance.StudentId == StudentId)
                    .Select(A => new AnswerEntity
                    {
                        Id = A.Id,
                        AnswerText = A.AnswerText,
                        PollInstanceId = A.PollInstanceId
                    }).ToList()
                }).ToList()
            }).ToList();


            return polls.Select(P => P.ToDomain()).ToList();
        }
    }
}
