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

        public async Task<BaseResponse> ImportPoll(PollDTO pollToCreate)
        {
            try
            {
                CreateComandResponse<Poll> createdPollResponse = CreatePoll(pollToCreate).Result;
                if (createdPollResponse.entity == null) return createdPollResponse;

                // Create components, variables and poll_variables (intermediate table)
                await CreateComponentsAndVariables(pollToCreate.Components, createdPollResponse.entity.Id);


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



        public async Task<CreateComandResponse<Poll>> CreatePoll(PollDTO pollToCreate)
        {
            try
            {
                CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = pollToCreate };
                return await _mediator.Send(createPollCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll: {ex.Message}");
                return new CreateComandResponse<Poll>(null, "Error", false);
            }
        }
        public async Task<CreateComandResponse<Component>> CreateComponent(ComponentDTO componentDto)
        {
            try
            {
                CreateComponentCommand createComponentCommand = new CreateComponentCommand() { Component = componentDto };
                return await _mediator.Send(createComponentCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateComandResponse<Component>(null, "Error", false);
            }
        }
        public async Task<CreateComandResponse<Variable>> CreateRelationshipPollVariable(VariableDTO variable, int asociatedPollId, int asociatedVariableId)
        {
            try
            {
                CreatePollVariableCommand createPollVariableCommand = new CreatePollVariableCommand()
                {
                    Variable = variable,
                    PollId = asociatedPollId,
                    VariableId = asociatedVariableId,
                };
                return await _mediator.Send(createPollVariableCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateComandResponse<Variable>(null, "Error", false);
            }
        }
        public async Task CreateVariables(ICollection<VariableDTO> variablesDtos, int asociatedPollId, int asociatedComponentId)
        {
            try
            {
                foreach (VariableDTO variableDto in variablesDtos)
                {
                    CreateVariableCommand createVariableCommand = new CreateVariableCommand()
                    {
                        Variable = variableDto,
                        PollId = asociatedPollId,
                        ComponentId = asociatedComponentId
                    };
                    CreateComandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);

                    if (createdVariable.Success)
                    {   // Add manual relationship between poll_variable
                        int asociatedVariableId = createdVariable.entity.Id;
                        CreateComandResponse<Variable> createdPollVariable = await CreateRelationshipPollVariable(variableDto, asociatedPollId, asociatedVariableId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
            }
        }
        public async Task CreateComponentsAndVariables(ICollection<ComponentDTO> componentDtoList, int asociatedPollId)
        {
            foreach (ComponentDTO componentDto in componentDtoList)
            {
                try
                {
                    CreateComandResponse<Component> createdComponent = await CreateComponent(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.entity.Id;
                        await CreateVariables(componentDto.Variables, asociatedPollId, asociatedComponentId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error creating component: " + ex.Message);
                }
            }

        }
    }
}