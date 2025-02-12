using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Commands.CreatePollInstance
{
    public class CreatePollInstanceCommandHandler
    {
        private readonly IComponentRepository _pollRepository;
        private readonly ILogger<CreatePollInstanceCommandHandler> _logger;

        public CreatePollInstanceCommandHandler(IComponentRepository pollRepository, ILogger<CreatePollInstanceCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(CreatePollInstanceCommand request, CancellationToken cancellationToken)
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
