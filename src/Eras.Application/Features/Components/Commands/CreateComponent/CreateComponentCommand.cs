using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Components.Commands.CreateCommand
{
    public class CreateComponentCommand : IRequest<CreateCommandResponse<Component>>
    {
        public ComponentDTO? Component;
    }
}
