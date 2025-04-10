using Eras.Application.Contracts.Persistence;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories;

public class StudentPollsRepository(AppDbContext Context) : BaseRepository<Poll, PollEntity>(Context, PollMapper.ToDomain, PollMapper.ToPersistence), IStudentPollsRepository
{
    public async Task<List<Poll>> GetPollsByStudentIdAsync(int StudentId)
    {
        var polls = _context.Polls
                .Where(Polls => Polls.PollVariables
                    .Any(PollVar => PollVar.Answers
                        .Any(Ans => Ans.PollInstance.StudentId == StudentId)))
                .Select(Poll => new
                {
                    Poll.Id,
                    Poll.Uuid,
                    Poll.Name,
                    Poll.Version
                })
                .Distinct()
                .ToList()
                .Select(Poll => new PollEntity
                {
                    Id = Poll.Id,
                    Uuid = Poll.Uuid,
                    Name = Poll.Name,
                    Version = Poll.Version,
                    PollVariables = [.. _context.PollVariables
                        .Where(PollVar => PollVar.PollId == Poll.Id)
                        .Select(PollVar => new PollVariableJoin
                        {
                            Id = PollVar.Id,
                            Answers = PollVar.Answers
                                .Where(Ans => Ans.PollInstance.StudentId == StudentId)
                                .Select(Ans => new AnswerEntity
                                {
                                    Id = Ans.Id,
                                    AnswerText = Ans.AnswerText,
                                    PollInstanceId = Ans.PollInstanceId
                                })
                                .ToList()
                        })]
                })
                .ToList();


        return [..polls.Select(P => P.ToDomain())];
    }
}
