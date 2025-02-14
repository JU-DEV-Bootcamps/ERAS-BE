using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Commands.CreatePollInstance
{
    public class CreatePollInstanceCommandHandler
    {
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<CreatePollInstanceCommandHandler> _logger;

        public CreatePollInstanceCommandHandler(IPollInstanceRepository pollInstanceRepository, ILogger<CreatePollInstanceCommandHandler> logger)
        {
            _pollInstanceRepository = pollInstanceRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<PollInstance>> Handle(CreatePollInstanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                PollInstance? pollInstance = request.PollInstance.ToDomain();
                PollInstance createdPoll = await _pollInstanceRepository.AddAsync(pollInstance);
                return new CreateComandResponse<PollInstance>(createdPoll, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: ");
                return new CreateComandResponse<PollInstance>(null, "Error", false);
            }
        }
    }
}
