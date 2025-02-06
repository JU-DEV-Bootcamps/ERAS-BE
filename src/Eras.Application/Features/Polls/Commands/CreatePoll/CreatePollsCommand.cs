using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Dtos;
using Eras.Application.Utils;
using MediatR;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollsCommand : IRequest<BaseResponse>
    {
        public PollDTO[] polls;
    }
}
