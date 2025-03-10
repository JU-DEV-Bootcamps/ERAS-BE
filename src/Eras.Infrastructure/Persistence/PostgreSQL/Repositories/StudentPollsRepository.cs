using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Joins;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    public class StudentPollsRepository : BaseRepository<Poll, PollEntity>, IStudentPollsRepository
    {

        public StudentPollsRepository(AppDbContext context)
            : base(context, Mappers.PollMapper.ToDomain, Mappers.PollMapper.ToPersistence)
        {
        }

        public async Task<List<Poll>> GetPollsByStudentIdAsync(int studentId)
        {
            var polls = _context.Polls
                    .Where(p => p.PollVariables
                        .Any(pv => pv.Answers
                            .Any(a => a.PollInstance.StudentId == studentId)))
                    .Select(p => new
                    {
                        p.Id,
                        p.Uuid,
                        p.Name,
                        p.Version
                    })
                    .Distinct()
                    .ToList()
                    .Select(p => new PollEntity
                    {
                        Id = p.Id,
                        Uuid = p.Uuid,
                        Name = p.Name,
                        Version = p.Version,
                        PollVariables = _context.PollVariables
                            .Where(pv => pv.PollId == p.Id)
                            .Select(pv => new PollVariableJoin
                            {
                                Id = pv.Id,
                                Answers = pv.Answers
                                    .Where(a => a.PollInstance.StudentId == studentId)
                                    .Select(a => new AnswerEntity
                                    {
                                        Id = a.Id,
                                        AnswerText = a.AnswerText,
                                        PollInstanceId = a.PollInstanceId
                                    })
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList();


            return polls.Select(p => p.ToDomain()).ToList();
        }
    }
}
