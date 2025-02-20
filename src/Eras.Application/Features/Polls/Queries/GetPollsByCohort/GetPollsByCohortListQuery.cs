using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQuery: IRequest<List<Poll>>
    {
        public int CohortId { get; set; }
    }
}
