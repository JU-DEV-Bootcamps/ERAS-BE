using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQuery : IRequest<List<Poll>>
    {
        public int CohortId { get; set; }
    }
}
