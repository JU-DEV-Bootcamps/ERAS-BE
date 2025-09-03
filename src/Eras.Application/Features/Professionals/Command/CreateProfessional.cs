using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Professionals.Commands.CreateProfessional
{
    public class CreateProfessionalCommand : IRequest<CreateCommandResponse<JUProfessional>>
    {
        public JUProfessionalDTO Professional = default!;
    }
}
