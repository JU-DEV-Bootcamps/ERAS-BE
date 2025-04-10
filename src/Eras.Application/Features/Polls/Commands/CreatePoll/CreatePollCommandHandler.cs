using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, CreateCommandResponse<Poll>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<CreatePollCommandHandler> _logger;

        public CreatePollCommandHandler(IPollRepository pollRepository, ILogger<CreatePollCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<Poll>> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Poll? pollInDB = await _pollRepository.GetByNameAsync(request.Poll.Name);
                if (pollInDB != null) return new CreateCommandResponse<Poll>(pollInDB, 0, "Success", true);

                Poll poll = request.Poll.ToDomain();
                poll.Uuid = Guid.NewGuid().ToString();
                Poll response = await _pollRepository.AddAsync(poll);
                return new CreateCommandResponse<Poll>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: " + request.Poll.Name);
                return new CreateCommandResponse<Poll>(null, 0, "Error", false);
            }
        }
    }
}
