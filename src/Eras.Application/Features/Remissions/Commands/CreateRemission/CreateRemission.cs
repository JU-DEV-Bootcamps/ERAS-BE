using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Remmisions.Commands.CreateRemission
{
    public class CreateRemissionCommand : IRequest<CreateCommandResponse<JURemission>>
    {
        public JURemissionDTO Remission = default!;
    }
}
