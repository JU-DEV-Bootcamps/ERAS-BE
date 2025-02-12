using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollsCommandHandler : IRequestHandler<CreatePollsCommand, BaseResponse>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<CreatePollsCommandHandler> _logger;
        private readonly IMediator _mediator;

        public CreatePollsCommandHandler(IPollRepository pollRepository, ILogger<CreatePollsCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }
        public async Task<BaseResponse> Handle(CreatePollsCommand request, CancellationToken cancellationToken)
        {
            var polls = request.polls;
            _logger.LogInformation("Creating Poll");
            return new BaseResponse(true);
        }
    }
}
