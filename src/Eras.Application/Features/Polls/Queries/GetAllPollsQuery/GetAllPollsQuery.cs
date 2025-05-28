using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetAllPollsQuery
{
    public class GetAllPollsQuery : IRequest<List<GetPollsQueryResponse>>;
}
