using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.PollInstances.Commands.CreatePollInstance
{
    public class CreatePollInstanceCommand : IRequest<CreateCommandResponse<PollInstance>>
    {
        public PollInstanceDTO? PollInstance;
    }
}
