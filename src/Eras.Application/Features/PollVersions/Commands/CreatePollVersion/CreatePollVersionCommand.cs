using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

namespace Eras.Application.Features.PollVersions.Commands.CreatePollVersion;
public class CreatePollVersionCommand: IRequest<CreateCommandResponse<PollVersion>>
{
    public required PollVersionDTO PollVersionDTO { get; set; }
    public int PollId { get; set; }
}
