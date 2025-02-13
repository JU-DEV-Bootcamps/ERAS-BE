using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Domain.Common;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, BaseResponse>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<CreatePollCommandHandler> _logger;

        public CreatePollCommandHandler(IPollRepository pollRepository, ILogger<CreatePollCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Poll poll = request.Poll.ToDomain();
                poll.Audit = new AuditInfo() { 
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                Poll response = await _pollRepository.AddAsync(poll);
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: " + request.Poll.Name);
                return new BaseResponse(false);
            }
        }
    }
}
