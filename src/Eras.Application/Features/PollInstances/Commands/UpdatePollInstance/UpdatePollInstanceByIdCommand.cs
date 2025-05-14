using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollInstances.Commands.UpdatePollInstance;
public class UpdatePollInstanceByIdCommand: IRequest<CreateCommandResponse<PollInstance>>
{
    public required PollInstanceDTO PollInstanceDTO { get; set; }
}
