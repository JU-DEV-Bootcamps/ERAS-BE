using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
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

namespace Eras.Application.Services
{
    public class PollOrchestratorService // Create interface
    {
        private readonly IMediator _mediator;
        ILogger<PollOrchestratorService> _logger;

        public PollOrchestratorService(IMediator mediator, 
            ILogger<PollOrchestratorService> logger
            )
        {
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<BaseResponse> CreatePoll(PollDTO pollToCreate)
        {
            try
            {
                // Create poll
                CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = pollToCreate };
                CreateComandResponse<Poll> createdPollResponse = await _mediator.Send(createPollCommand);
                if (createdPollResponse.entity == null)
                {
                    _logger.LogError("Error creating poll: ");
                    return createdPollResponse;
                }
                
                 // Create components                 
                foreach (ComponentDTO componentDto in pollToCreate.Components)
                { 
                    try
                    {
                        CreateComponentCommand createComponentCommand = new CreateComponentCommand() { Component = componentDto };
                        CreateComandResponse<Component> createdComponent = await _mediator.Send(createComponentCommand);

                        if (createdComponent.Success)

                        {
                            // Create variables
                            foreach (VariableDTO variable in componentDto.Variables)
                            {
                                CreateVariableCommand createVariableCommand = new CreateVariableCommand() { 
                                    Variable = variable,
                                    PollId = createdPollResponse.entity.Id, 
                                    ComponentId = createdComponent.entity.Id
                                };
                                CreateComandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);
                                if (createdVariable.Success)
                                {

                                    CreatePollVariableCommand createPollVariableCommand = new CreatePollVariableCommand() { 
                                        Variable = variable,
                                        PollId = createdPollResponse.entity.Id,
                                        VariableId = createdVariable.entity.Id,
                                    };
                                    CreateComandResponse<Variable> createdPollVariable= await _mediator.Send(createPollVariableCommand);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error creating component: " + ex.Message);
                    }
                }

                // Create poll instances
                // Create asnswers
                // Create students

                return new BaseResponse(createdPollResponse.Success);
            }
            catch (Exception ex)
            {
                return new BaseResponse( "Error during import process: " +ex.Message, false);
            }
        }
    }
}