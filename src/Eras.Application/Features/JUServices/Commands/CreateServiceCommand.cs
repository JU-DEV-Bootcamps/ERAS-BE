
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.JUServices.Commands.CreateJUService
{
    public class CreateJUServiceCommand : IRequest<CreateCommandResponse<JUService>>
    {
        public JUServiceDTO Service = default!;
    }
}
