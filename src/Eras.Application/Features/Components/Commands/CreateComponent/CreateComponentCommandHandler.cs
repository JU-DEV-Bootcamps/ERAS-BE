using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Components.Commands.CreateCommand
{
    public class CreateComponentCommandHandler
    {
        private readonly IComponentRepository _pollRepository;
        private readonly ILogger<CreateComponentCommandHandler> _logger;

        public CreateComponentCommandHandler(IComponentRepository pollRepository, ILogger<CreateComponentCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // var poll = CosmicLatteMapper.DtoToPoll(request.poll);
                // await _pollRepository.AddAsync(poll);
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: ");
                return new BaseResponse(false);
            }
        }
    }
}
