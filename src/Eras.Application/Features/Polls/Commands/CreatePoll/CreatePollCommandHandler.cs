using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Mappers;
using Eras.Application.Contracts.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Entities;
using Eras.Domain.Common;
using Eras.Application.Models;
using Eras.Application.Dtos;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, CreateComandResponse<Poll>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<CreatePollCommandHandler> _logger;

        public CreatePollCommandHandler(IPollRepository pollRepository, ILogger<CreatePollCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<Poll>> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Poll? pollInDB = await _pollRepository.GetByNameAsync(request.Poll.Name);
                if (pollInDB != null) return new CreateComandResponse<Poll>(pollInDB, 0, "Success", true);

                Poll poll = request.Poll.ToDomain();
                poll.Uuid = Guid.NewGuid().ToString();
                Poll response = await _pollRepository.AddAsync(poll);
                return new CreateComandResponse<Poll>(response,1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: " + request.Poll.Name);
                return new CreateComandResponse<Poll>(null,0, "Error", false);
            }
        }
    }
}
