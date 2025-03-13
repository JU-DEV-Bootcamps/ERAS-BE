using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eras.Application.Features.PollInstances.Commands.CreatePollInstance
{
    public class CreatePollInstanceCommandHandler : IRequestHandler<CreatePollInstanceCommand, CreateCommandResponse<PollInstance>>
    {
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<CreatePollInstanceCommandHandler> _logger;

        public CreatePollInstanceCommandHandler(IPollInstanceRepository pollInstanceRepository, ILogger<CreatePollInstanceCommandHandler> logger)
        {
            _pollInstanceRepository = pollInstanceRepository;
            _logger = logger;
        }

        public async Task<CreateCommandResponse<PollInstance>> Handle(CreatePollInstanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                PollInstance? pollInstanceDB = await _pollInstanceRepository.GetByUuidAndStudentIdAsync(request.PollInstance.Uuid, request.PollInstance.Student.Id);
                if (pollInstanceDB != null) return new CreateCommandResponse<PollInstance>(pollInstanceDB, 0, "Success", true);

                PollInstance? pollInstance = request.PollInstance.ToDomain();
                PollInstance createdPoll = await _pollInstanceRepository.AddAsync(pollInstance);
                return new CreateCommandResponse<PollInstance>(createdPoll,1, "Success", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the poll: ");
                return new CreateCommandResponse<PollInstance>(null,0, "Error", false);
            }
        }
    }
}
