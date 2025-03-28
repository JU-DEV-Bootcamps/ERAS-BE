﻿using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Cohort.Commands.CreateCohort;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.CreateStudentCohort;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Mappers;
using Eras.Application.Models;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Common;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Variable = Eras.Domain.Entities.Variable;
using Component = Eras.Domain.Entities.Component;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;

namespace Eras.Application.Services
{
    public class PollOrchestratorService // Create interface or comand
    {
        private readonly IMediator _mediator;
        ILogger<PollOrchestratorService> _logger;

        public PollOrchestratorService(IMediator mediator, ILogger<PollOrchestratorService> logger )
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<CreateCommandResponse<CreatedPollDTO>> ImportPollInstances(List<PollDTO> pollsToCreate)
        {
            try
            {
                // Create poll
                PollDTO pollDTO = pollsToCreate[0];
                CreateCommandResponse<Poll> createdPollResponse = await CreatePoll(pollDTO);
                int createdPollsInstances = 0;
                CreatedPollDTO createdPoll = new CreatedPollDTO();

                if (createdPollResponse.Entity != null)
                {
                    // Create components, variables and poll_variables (intermediate table)
                    List<Component> createdComponents = await CreateComponentsAndVariables(pollsToCreate[0].Components, createdPollResponse.Entity.Id);

                    foreach (PollDTO pollToCreate in pollsToCreate)
                    {
                        // Create students
                        CreateCommandResponse<Student> createdStudent = await CreateStudentFromPoll(pollToCreate);
                        createdPoll.studentDTOs.Add(createdStudent.Entity.ToDto());
                        // Create poll instances
                        CreateCommandResponse<PollInstance> createdPollInstance = await CreatePollInstance(createdStudent.Entity, createdPollResponse.Entity.Uuid, pollToCreate.FinishedAt);

                        // Create asnswers
                        if (createdPollInstance.Success)
                        {
                            await CreateAnswers(pollToCreate, createdComponents, createdPollInstance);
                        }
                        createdPollsInstances++;
                    }
                }
                return new CreateCommandResponse<CreatedPollDTO>(createdPoll, createdPollsInstances, "Success", true);
            }
            catch (Exception ex)
            {
                return new CreateCommandResponse<CreatedPollDTO>(null, 0, $"Error during import process {ex.Message}", false);
            }
        }
        public async Task<CreateCommandResponse<PollInstance>> CreatePollInstance(Student student, string pollUuid, DateTime finishedAt)
        {
            try
            {
                PollInstance pollInstance = new PollInstance() { Uuid = pollUuid, Student = student, FinishedAt = finishedAt};

                pollInstance.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreatePollInstanceCommand createPollInstanceCommand = new CreatePollInstanceCommand() { PollInstance = pollInstance.ToDTO()};
                return await _mediator.Send(createPollInstanceCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll instance: {ex.Message}");
                return new CreateCommandResponse<PollInstance>(null, 0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetail(int studentId)
        {
            GetStudentDetailByStudentIdQuery query = new GetStudentDetailByStudentIdQuery()
            {
                StudentId = studentId
            };
            GetQueryResponse<StudentDetail> createdStudentDetail = await _mediator.Send(query);
            if(createdStudentDetail.Success && createdStudentDetail.Body != null)
            {
                CreateCommandResponse<StudentDetail> command = new CreateCommandResponse<StudentDetail>(createdStudentDetail.Body,
                    0, createdStudentDetail.Message, true);
                return command;
            }
            else
            {
                StudentDetailDTO studentDetailDTO = new StudentDetailDTO() { StudentId = studentId };
                studentDetailDTO.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreateStudentDetailCommand createStudentDetailCommand = new CreateStudentDetailCommand() { StudentDetailDto = studentDetailDTO };
                return await _mediator.Send(createStudentDetailCommand);
            }
        }
        public async Task<CreateCommandResponse<Cohort>> CreateAndSetStudentCohort(StudentDTO studentDto,CohortDTO cohort)
        {
            cohort.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateCohortCommand createCohortCommand = new CreateCohortCommand() { CohortDto = cohort };
            CreateCommandResponse <Cohort> createdCohort = await _mediator.Send(createCohortCommand);

            if (createdCohort.Success)
            {
                CreateStudentCohortCommand createStudentCohortCommand = new CreateStudentCohortCommand() { CohortId = createdCohort.Entity.Id, StudentId = studentDto.Id };
                CreateCommandResponse<Student> createdStudentCohort = await _mediator.Send(createStudentCohortCommand);
            }

            return createdCohort;
        }
        public async Task<CreateCommandResponse<Student>> CreateStudentFromPoll(PollDTO pollToCreate)
        {
            try
            {
                StudentDTO studentToCreate = (pollToCreate.Components.FirstOrDefault()?.Variables.FirstOrDefault()?.Answer?.Student)
                    ?? throw new ArgumentNullException("Student information not found");
                studentToCreate.Uuid = Guid.NewGuid().ToString();
                studentToCreate.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic Latte Import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentToCreate };
                CreateCommandResponse<Student> createdStudent = await _mediator.Send(createStudentCommand);

                if (createdStudent.Success)
                {
                    CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetail(createdStudent.Entity.Id);
                    CohortDTO cohortToCreate = studentToCreate.Cohort;
                    createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                    CreateCommandResponse<Cohort> createdCohort = await CreateAndSetStudentCohort(createdStudent.Entity.ToDto(), cohortToCreate);
                    createdStudent.Entity.Cohort = createdCohort.Entity;
                }
                return createdStudent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating student: {ex.Message}");
                return new CreateCommandResponse<Student>(null,0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<Poll>> CreatePoll(PollDTO pollToCreate)
        {
            try
            {
                pollToCreate.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = pollToCreate };
                return await _mediator.Send(createPollCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll: {ex.Message}");
                return new CreateCommandResponse<Poll>(null, 0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<Variable>> CreateRelationshipPollVariable(VariableDTO variable, int asociatedPollId, int asociatedVariableId)
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
                return new CreateCommandResponse<Variable>(null, 0, "Error", false);
            }
        }
        public async Task<List<Variable>> CreateVariables(ICollection<VariableDTO> variablesDtos, int asociatedPollId, int asociatedComponentId)
        {
            try
            {
                List <Variable> createdVariables = new List <Variable>();
                if (variablesDtos == null) return createdVariables;

                foreach (VariableDTO variableDto in variablesDtos)
                {
                    variableDto.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    CreateVariableCommand createVariableCommand = new CreateVariableCommand()
                    {
                        Variable = variableDto,
                        PollId = asociatedPollId,
                        ComponentId = asociatedComponentId
                    };
                    CreateCommandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);

                    if (createdVariable.Success)
                    {   // Add manual relationship between poll_variable
                        int asociatedVariableId = createdVariable.Entity.Id;
                        CreateCommandResponse<Variable> createdPollVariable = await CreateRelationshipPollVariable(variableDto, asociatedPollId, asociatedVariableId);
                        createdVariable.Entity.PollVariableId = createdPollVariable.Entity.PollVariableId;
                        createdVariables.Add(createdVariable.Entity);
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
        public async Task CreateAnswers(PollDTO pollToCreate, List<Component> createdComponents, CreateCommandResponse<PollInstance> createdPollInstance)
        {
            var componentDict = createdComponents.ToDictionary(c => c.Name, c => c);

            List<AnswerDTO> answersToCreate = [];

            foreach (ComponentDTO component in pollToCreate.Components)
            {
                componentDict.TryGetValue(component.Name, out var componentByName);
                if (componentByName != null)
                {
                    var variableDict = componentByName.Variables.ToDictionary(v => v.Name, v => v);

                    foreach (VariableDTO variable in component.Variables)
                    {
                        try
                        {
                            if (variableDict.TryGetValue(variable.Name, out var variableByName))
                            {
                                AnswerDTO answerToCreate = variable.Answer != null ? variable.Answer : new AnswerDTO();
                                answerToCreate.PollVariableId = variableByName.PollVariableId;
                                answerToCreate.PollInstanceId = createdPollInstance.Entity.Id;
                                answerToCreate.Audit = new AuditInfo()
                                {
                                    CreatedBy = "Cosmic latte import",
                                    CreatedAt = DateTime.UtcNow,
                                    ModifiedAt = DateTime.UtcNow,
                                };
                                answersToCreate.Add(answerToCreate);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error creating answer: {ex.Message}");
                        }
                    }
                }
            }
            CreateAnswerListCommand createAnswerListCommand = new CreateAnswerListCommand() { Answers = answersToCreate };
            await _mediator.Send(createAnswerListCommand);
        }
        public async Task<CreateCommandResponse<Component>> CreateComponent(ComponentDTO componentDto)
        {
            try
            {
                componentDto.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                CreateComponentCommand createComponentCommand = new CreateComponentCommand() { Component = componentDto };
                return await _mediator.Send(createComponentCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateCommandResponse<Component>(null, 0, "Error", false);
            }
        }
        public async Task<List<Component>> CreateComponentsAndVariables(ICollection<ComponentDTO> componentDtoList, int asociatedPollId)
        {
            List<Component> createdComponents = [];
            foreach (ComponentDTO componentDto in componentDtoList)
            {
                try
                {
                    CreateCommandResponse<Component> createdComponent = await CreateComponent(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.Entity.Id;
                        List<Variable> createdVariables = await CreateVariables(componentDto.Variables, asociatedPollId, asociatedComponentId);
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
    }
}
