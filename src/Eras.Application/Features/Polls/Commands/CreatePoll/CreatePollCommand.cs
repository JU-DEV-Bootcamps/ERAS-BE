using Eras.Application.Dtos;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommand : IRequest<CreateCommandResponse<Poll>>
    {
        public PollDTO Poll = default!;
    }
}
