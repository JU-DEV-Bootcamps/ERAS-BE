using Eras.Application.Models.Response.Controllers.PollsController;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Queries.GetPollsByStudent
{
    public class GetPollsByStudentQuery : IRequest<List<GetPollsQueryResponse>>
    {
        public int StudentId { get; set; }
    }
}
