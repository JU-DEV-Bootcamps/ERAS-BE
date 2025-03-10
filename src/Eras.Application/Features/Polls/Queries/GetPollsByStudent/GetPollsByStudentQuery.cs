using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollsByStudent
{
    public class GetPollsByStudentQuery : IRequest<List<Poll>>
    {
        public int StudentId { get; set; }
    }
}
