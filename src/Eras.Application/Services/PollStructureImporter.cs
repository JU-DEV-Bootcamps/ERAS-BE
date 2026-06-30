using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Components.Queries.GetByNameAndPoll;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Application.Features.Variables.Commands.CreatePollVariableList;
using Eras.Application.Features.Variables.Commands.CreateVariableList;
using Eras.Application.Features.Variables.Queries.GetWithNameAndPollId;
using Eras.Application.Models.CommandsDTOS;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

using Component = Eras.Domain.Entities.Component;
using Variable = Eras.Domain.Entities.Variable;

namespace Eras.Application.Services
{
    /// <summary>
    /// Imports the poll template structure: poll, components, variables and the poll-variable
    /// join rows, tracking version changes through the shared <see cref="ImportContext"/>.
    /// </summary>
    public class PollStructureImporter
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PollStructureImporter> _logger;

        public PollStructureImporter(IMediator Mediator, ILogger<PollStructureImporter> Logger)
        {
            _mediator = Mediator;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<Poll>> CreatePollAsync(PollDTO PollToCreate, ImportContext Context)
        {
            try
            {
                PollToCreate.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                GetPollByNameQuery getPollByNameQuery = new GetPollByNameQuery() { pollName = PollToCreate.Name };
                var pollByName = await _mediator.Send(getPollByNameQuery);
                if (pollByName.Success && pollByName.Status == QueryEnums.QueryResultStatus.NotFound)
                {
                    Context.IsNewPoll = true;
                    Context.VersionNumber = 1;
                    PollToCreate.LastVersion = Context.VersionNumber;
                    PollToCreate.LastVersionDate = Context.InitDate;
                    CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = PollToCreate };
                    return await _mediator.Send(createPollCommand);
                }
                Context.VersionNumber = pollByName.Body.LastVersion;
                return new CreateCommandResponse<Poll>(pollByName.Body, 1, pollByName.Message, pollByName.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll: {ex.Message}");
                return new CreateCommandResponse<Poll>(new Poll(), 0, "Error", false);
            }
        }

        public async Task<List<Component>> CreateComponentsAndVariablesAsync(ICollection<ComponentDTO> ComponentDtoList, int AsociatedPollId, ImportContext Context)
        {
            List<Component> createdComponents = [];
            foreach (ComponentDTO componentDto in ComponentDtoList)
            {
                try
                {
                    if (!Context.IsNewPoll)
                    {
                        var componentOldQuery = new GetComponentByNameAndPollIdQuery()
                        {
                            ComponentName = componentDto.Name,
                            PollId = AsociatedPollId
                        };
                        var componentOld = await _mediator.Send(componentOldQuery);
                        if (componentOld.Status == QueryEnums.QueryResultStatus.NotFound && !Context.IsNewVersion)
                        {
                            Context.VersionNumber += 1;
                            Context.IsNewVersion = true;
                        }
                    }
                    CreateCommandResponse<Component> createdComponent = await CreateComponentAsync(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.Entity!.Id;
                        List<Variable> createdVariables = await CreateVariablesAsync(componentDto.Variables, AsociatedPollId, asociatedComponentId, Context);
                        createdComponent.Entity.Variables = createdVariables;
                        createdComponents.Add(createdComponent.Entity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error creating component: " + ex.Message);
                }
            }
            return createdComponents;
        }

        public async Task<CreateCommandResponse<Component>> CreateComponentAsync(ComponentDTO ComponentDto)
        {
            try
            {
                ComponentDto.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreateComponentCommand createComponentCommand = new CreateComponentCommand() { Component = ComponentDto };
                return await _mediator.Send(createComponentCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateCommandResponse<Component>(null, 0, "Error", false);
            }
        }

        public async Task<List<Variable>> CreateVariablesAsync(ICollection<VariableDTO> VariablesDtos, int AsociatedPollId, int AsociatedComponentId, ImportContext Context)
        {
            try
            {
                List<Variable> createdVariables = new List<Variable>();
                if (VariablesDtos == null) return createdVariables;

                var query = new GetVariablesWithNameAndPollIdQuery() { PollId = AsociatedPollId };
                GetQueryResponse<List<Variable>> responseQuery = await _mediator.Send(query);
                List<Variable> existingVariables = responseQuery.Body;

                List<VariableListCommandDTO> variableListCommandDTOs = [];

                foreach (VariableDTO variableDto in VariablesDtos)
                {
                    variableDto.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    if (!Context.IsNewPoll && responseQuery.Status != QueryEnums.QueryResultStatus.NotFound)
                    {
                        var variableAlreadyExists = existingVariables.Any(
                            ExistingVar => ExistingVar.Name == variableDto.Name && ExistingVar.IdPoll == AsociatedPollId
                        );
                        if (!variableAlreadyExists && !Context.IsNewVersion)
                        {
                            Context.VersionNumber += 1;
                            Context.IsNewVersion = true;
                        }
                    }

                    variableListCommandDTOs.Add(new VariableListCommandDTO()
                    {
                        variable = variableDto,
                        PollId = AsociatedPollId,
                        ComponentId = AsociatedComponentId
                    });
                }
                CreateVariableListCommand createVariableListCommand = new CreateVariableListCommand()
                {
                    Variables = variableListCommandDTOs
                };
                CreateCommandResponse<List<Variable>> createdVariablesList = await _mediator.Send(createVariableListCommand);
                if (createdVariablesList.Success && createdVariablesList.Entity != null)
                {
                    CreateCommandResponse<List<Variable>> createdPollVariables = await CreateRelationshipsPollVariablesAsync(createdVariablesList.Entity, AsociatedPollId, Context);
                    if (createdPollVariables.Success && createdPollVariables.Entity != null)
                    {
                        foreach (Variable createdVariable in createdVariablesList.Entity)
                        {
                            Variable? createdPollVariable = createdPollVariables.Entity.FirstOrDefault(PollVar => createdVariable.Id == PollVar.Id);
                            if (createdPollVariable != null)
                            {
                                createdVariable.PollVariableId = createdPollVariable.PollVariableId;
                            }

                            createdVariables.Add(createdVariable);
                        }
                    }
                }
                return createdVariables;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating variable: {ex.Message}");
                return [];
            }
        }

        public async Task<CreateCommandResponse<List<Variable>>> CreateRelationshipsPollVariablesAsync(List<Variable> Variables, int AsociatedPollId, ImportContext Context)
        {
            try
            {
                foreach (Variable variable in Variables)
                {
                    variable.Version = new VersionInfo()
                    {
                        VersionNumber = Context.VersionNumber,
                        VersionDate = Context.InitDate
                    };
                }

                CreatePollVariableListCommand createPollVariableListCommand = new CreatePollVariableListCommand()
                {
                    Variables = new PollVariableListCommandDTO()
                    {
                        variables = Variables,
                        pollId = AsociatedPollId
                    }
                };
                return await _mediator.Send(createPollVariableListCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateCommandResponse<List<Variable>>(null, 0, "Error", false);
            }
        }
    }
}
