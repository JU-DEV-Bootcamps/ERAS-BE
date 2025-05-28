using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollsByCohort
{
    public class GetPollsByCohortListQuery: IRequest<List<GetPollsQueryResponse>>
    {
        public int CohortId { get; set; }
    }
}
