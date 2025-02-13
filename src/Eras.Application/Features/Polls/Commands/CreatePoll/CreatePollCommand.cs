using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
using Eras.Application.Utils;
using Eras.Domain.Entities;
using MediatR;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommand : IRequest<BaseResponse>
    {
        public PollDTO Poll = default!;
    }
}
