using Eras.Application.Contracts.Persistence;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Polls.Commands.CreatePoll
{
    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, CreateCommandResponse<Poll>>
    {
        private readonly IPollRepository _pollRepository;
        private readonly ILogger<CreatePollCommandHandler> _logger;

        public CreatePollCommandHandler(IPollRepository PollRepository, ILogger<CreatePollCommandHandler> Logger)
        {
            _pollRepository = PollRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Poll>> Handle(CreatePollCommand Request, CancellationToken CancellationToken)
        {
            try
            {
                Poll? pollInDB = await _pollRepository.GetByNameAsync(Request.Poll.Name);
                if (pollInDB != null) return new CreateCommandResponse<Poll>(new Poll(), "Entity already exists", false,
                    CommandEnums.CommandResultStatus.AlreadyExists);

                Poll poll = Request.Poll.ToDomain();
                poll.Uuid = Guid.NewGuid().ToString();
                Poll response = await _pollRepository.AddAsync(poll);
                return new CreateCommandResponse<Poll>(response, 1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: " + Request.Poll.Name);
                return new CreateCommandResponse<Poll>(new Poll(), "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }
    }
}
