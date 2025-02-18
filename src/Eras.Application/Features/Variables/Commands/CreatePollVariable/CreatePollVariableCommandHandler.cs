using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Features.Variables.Commands.CreatePollVariable
{
    internal class CreatePollVariableCommandHandler : IRequestHandler<CreatePollVariableCommand, CreateComandResponse<Variable>>
    {
        private readonly IPollVariableRepository _pollVariableRepository;
        private readonly ILogger<CreatePollVariableCommandHandler> _logger;

        public CreatePollVariableCommandHandler(
            IPollVariableRepository pollVariableRepository,
            ILogger<CreatePollVariableCommandHandler> logger)
        {

            _pollVariableRepository = pollVariableRepository;
            _logger = logger;
        }

        public async Task<CreateComandResponse<Variable>> Handle(CreatePollVariableCommand request, CancellationToken cancellationToken)
        {

            try
            {
                Variable createdVariable = request.Variable.ToDomain();
                createdVariable.IdPoll = request.PollId;
                createdVariable.Id = request.VariableId;
                Variable response = await _pollVariableRepository.AddAsync(createdVariable);
                return new CreateComandResponse<Variable>(response,1, "Success", true);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred creating the variable: ");
                return new CreateComandResponse<Variable>(null,0, "Error", false);
            }
        }
    }
}
