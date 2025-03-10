using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Components.Queries
{
    public class GetComponentsAvgByStudentQuery : IRequest<List<ComponentsAvg>>
    {
        public int StudentId { get; set; }
        public int PollId { get; set; }
    }
}
