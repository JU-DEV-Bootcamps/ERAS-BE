using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Answers.Queries
{
    public class GetStudentAnswersByPollQuery : IRequest<List<StudentAnswer>>
    {
        public int StudentId { get; set; }
        public int PollId { get; set; }
    }
}
