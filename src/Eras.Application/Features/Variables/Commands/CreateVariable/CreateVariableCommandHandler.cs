using System;
using System.Collections.Generic;
using System.Linq;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Utils;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.Variables.Commands.CreateVariable
{
    public class CreateVariableCommandHandler
    {
        private readonly IComponentRepository _pollRepository;
        private readonly ILogger<CreateVariableCommandHandler> _logger;

        public CreateVariableCommandHandler(IComponentRepository pollRepository, ILogger<CreateVariableCommandHandler> logger)
        {
            _pollRepository = pollRepository;
            _logger = logger;
        }

        public async Task<BaseResponse> Handle(CreateVariableCommand request, CancellationToken cancellationToken)
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
