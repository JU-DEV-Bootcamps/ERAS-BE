using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Utils;
using MediatR;

namespace Eras.Application.Features.PollInstances.Commands.CreatePollInstance
{
    public class CreatePollInstanceCommand : IRequest<BaseResponse>
    {
        public PollDTO? component;
    }
}
