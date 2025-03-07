using Eras.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery
{
    public class GetAllPollsQuery : IRequest<List<Poll>>;
}
