using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Exceptions;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Eras.Application.Features.Cohorts.Commands.CreateCohort;
using Eras.Application.Features.Components.Commands.CreateCommand;
using Eras.Application.Features.Components.Queries.GetByNameAndPoll;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.PollInstances.Commands.UpdatePollInstance;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Features.Polls.Commands.UpdatePoll;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Application.Features.Students.Commands.CreateStudent;
using Eras.Application.Features.Students.Commands.CreateStudentCohort;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Application.Features.StudentsDetails.Commands.CreateStudentDetail;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;
using Eras.Application.Features.Variables.Commands.CreatePollVariable;
using Eras.Application.Features.Variables.Commands.CreateVariable;
using Eras.Application.Features.Variables.Queries.GetByNameAndPollId;
using Eras.Application.Mappers;
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
    public class PollOrchestratorService
    {
        private readonly IMediator _mediator;
        ILogger<PollOrchestratorService> _logger;
        private bool IsNewPoll;
        private int VersionNumber;
        private bool IsNewVersion = false;
        private DateTime InitDate; 

        public PollOrchestratorService(IMediator Mediator, ILogger<PollOrchestratorService> Logger)
        {
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<CreatedPollDTO>> ImportPollInstancesAsync(List<PollDTO> PollsToCreate)
        {
            try
            {
                InitDate = DateTime.UtcNow;
                // Create poll
                PollDTO pollDTO = PollsToCreate[0];
                CreateCommandResponse<Poll> createdPollResponse = await CreatePollAsync(pollDTO);
                int createdPollsInstances = 0;
                CreatedPollDTO createdPoll = new CreatedPollDTO();

                if (createdPollResponse.Success && createdPollResponse.Entity != null)
                {
                    var pollToUse = createdPollResponse.Entity.ToDto();
                    // Create components, variables and poll_variables (intermediate table)
                    List<Component> createdComponents = await CreateComponentsAndVariablesAsync(PollsToCreate[0].Components, 
                        createdPollResponse.Entity.Id);
                    if (IsNewVersion) 
                    {
                        pollToUse.LastVersion = VersionNumber;
                        pollToUse.LastVersionDate = InitDate;
                        var updateCommand = new UpdatePollByIdCommand() { PollDTO = pollToUse };
                        await _mediator.Send(updateCommand);
                    }
                    foreach (PollDTO pollToCreate in PollsToCreate)
                    {
                        // Create students
                        CreateCommandResponse<Student> createdStudent = await CreateStudentFromPollAsync(pollToCreate);
                        if (createdStudent.Success && createdStudent.Entity != null)
                        {
                            createdPoll.studentDTOs.Add(createdStudent.Entity.ToDto());
                            // Create poll instances
                            CreateCommandResponse<PollInstance> createdPollInstance = await CreatePollInstanceAsync(createdStudent.Entity, 
                                createdPollResponse.Entity.Uuid, pollToCreate.FinishedAt);
                            // Create asnswers
                            if (createdPollInstance.Success)
                            {
                                await CreateAnswersAsync(pollToCreate, createdComponents, createdPollInstance);
                            }
                            createdPollsInstances++;
                        }
                    }
                }
                return new CreateCommandResponse<CreatedPollDTO>(createdPoll, createdPollsInstances, "Success", true);
            }
            catch (Exception ex)
            {
                return new CreateCommandResponse<CreatedPollDTO>(null, 0, $"Error during import process {ex.Message}", false);
            }
        }
        public async Task<CreateCommandResponse<PollInstance>> CreatePollInstanceAsync(Student Student, string PollUuid, DateTime FinishedAt)
        {
            try
            {
                var queryPollInstance = new GetPollInstanceByUuidAndStudentIdQuery()
                {
                    PollUuid = PollUuid,
                    StudentId = Student.Id,
                };
                var responseQuery = await _mediator.Send(queryPollInstance);
                if (responseQuery.Status == QueryEnums.QueryResultStatus.Success)
                {
                    if (IsNewVersion)
                    {
                        responseQuery.Body.LastVersion = VersionNumber;
                        responseQuery.Body.LastVersionDate = InitDate;
                        var updateCommand = new UpdatePollInstanceByIdCommand() { PollInstanceDTO = responseQuery.Body.ToDTO() };
                        var responseUpdate = await _mediator.Send(updateCommand);
                        return responseUpdate;
                    }
                    else
                        return new  CreateCommandResponse<PollInstance>(responseQuery.Body,responseQuery.Message,responseQuery.Success);
                }
                else
                {
                    PollInstance pollInstance = new PollInstance() { Uuid = PollUuid, Student = Student, FinishedAt = FinishedAt };

                    pollInstance.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    pollInstance.LastVersion = VersionNumber;
                    pollInstance.LastVersionDate = InitDate;
                    CreatePollInstanceCommand createPollInstanceCommand = new CreatePollInstanceCommand() { PollInstance = pollInstance.ToDTO() };
                    return await _mediator.Send(createPollInstanceCommand);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll instance: {ex.Message}");
                return new CreateCommandResponse<PollInstance>(new PollInstance(), 0, "Error", false,CommandEnums.CommandResultStatus.Error);
            }
        }
        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetailAsync(int StudentId)
        {
            GetStudentDetailByStudentIdQuery query = new GetStudentDetailByStudentIdQuery()
            {
                StudentId = StudentId
            };
            GetQueryResponse<StudentDetail?> createdStudentDetail = await _mediator.Send(query);
            if (createdStudentDetail.Success && createdStudentDetail.Body != null)
            {
                CreateCommandResponse<StudentDetail> command = new CreateCommandResponse<StudentDetail>(createdStudentDetail.Body,
                    0, createdStudentDetail.Message, true);
                return command;
            }
            else
            {
                StudentDetailDTO studentDetailDTO = new StudentDetailDTO() { StudentId = StudentId };
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
        public async Task<CreateCommandResponse<Cohort>> CreateAndSetStudentCohortAsync(StudentDTO StudentDto, CohortDTO Cohort)
        {
            Cohort.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateCohortCommand createCohortCommand = new CreateCohortCommand() { CohortDto = Cohort };
            CreateCommandResponse<Cohort> createdCohort = await _mediator.Send(createCohortCommand);

            if (createdCohort.Success && createdCohort.Entity != null)
            {
                CreateStudentCohortCommand createStudentCohortCommand = new CreateStudentCohortCommand()
                { 
                    CohortId = createdCohort.Entity.Id,
                    StudentId = StudentDto.Id
                };
                CreateCommandResponse<Student> createdStudentCohort = await _mediator.Send(createStudentCohortCommand);
            }

            return createdCohort;
        }
        public async Task<CreateCommandResponse<Student>> CreateStudentFromPollAsync(PollDTO PollToCreate)
        {
            try
            {
                StudentDTO studentToCreate = (PollToCreate.Components.FirstOrDefault()?.Variables.FirstOrDefault()?.Answer?.Student)
                    ?? throw new ArgumentNullException("Student information not found");
                studentToCreate.Uuid = Guid.NewGuid().ToString();
                studentToCreate.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic Latte Import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                GetStudentByEmailQuery getStudentByEmailQuery = new GetStudentByEmailQuery() { studentEmail = studentToCreate.Email };
                CreateCommandResponse<Student> createdStudent;
                try
                {
                    GetQueryResponse<Student> getStudentResponse = await _mediator.Send(getStudentByEmailQuery);
                    createdStudent = new CreateCommandResponse<Student>(getStudentResponse.Body, "Success", true);
                }
                catch (EntityNotFoundException)
                {
                    CreateStudentCommand createStudentCommand = new CreateStudentCommand() { StudentDTO = studentToCreate };
                    createdStudent = await _mediator.Send(createStudentCommand);
                }
                if (createdStudent.Success && createdStudent.Entity != null)
                {
                    CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetailAsync(createdStudent.Entity.Id);
                    CohortDTO? cohortToCreate = studentToCreate.Cohort;

                    if (createdStudentDetail.Entity != null && cohortToCreate != null)
                    {
                        createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                        CreateCommandResponse<Cohort> createdCohort = await CreateAndSetStudentCohortAsync(createdStudent.Entity.ToDto(), cohortToCreate);
                        createdStudent.Entity.Cohort = createdCohort.Entity;
                    }
                    else
                    {
                        _logger.LogWarning($"Warning creating student: createdStudentDetail or cohortToCreate is null");
                    }
                }
                return createdStudent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating student: {ex.Message}");
                return new CreateCommandResponse<Student>(null, 0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<Poll>> CreatePollAsync(PollDTO PollToCreate)
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
                    IsNewPoll = true;
                    VersionNumber = 1;
                    PollToCreate.LastVersion = VersionNumber;
                    PollToCreate.LastVersionDate = InitDate;
                    CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = PollToCreate };
                    return await _mediator.Send(createPollCommand);
                }
                VersionNumber = pollByName.Body.LastVersion;
                return new CreateCommandResponse<Poll>(pollByName.Body, 1,pollByName.Message,pollByName.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll: {ex.Message}");
                return new CreateCommandResponse<Poll>(new Poll(), 0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<Variable>> CreateRelationshipPollVariableAsync(VariableDTO Variable, int AsociatedPollId, int AsociatedVariableId)
        {
            try
            {
                Variable.Version = new VersionInfo()
                {
                    VersionNumber = VersionNumber,
                    VersionDate = InitDate,
                };
                CreatePollVariableCommand createPollVariableCommand = new CreatePollVariableCommand()
                {
                    Variable = Variable,
                    PollId = AsociatedPollId,
                    VariableId = AsociatedVariableId,
                };
                return await _mediator.Send(createPollVariableCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
                return new CreateCommandResponse<Variable>(null, 0, "Error", false);
            }
        }
        public async Task<List<Variable>> CreateVariablesAsync(ICollection<VariableDTO> VariablesDtos, int AsociatedPollId, int AsociatedComponentId)
        {
            try
            {
                List<Variable> createdVariables = new List<Variable>();
                if (VariablesDtos == null) return createdVariables;

                foreach (VariableDTO variableDto in VariablesDtos)
                {
                    variableDto.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    if (!IsNewPoll)
                    {
                        var query = new GetVariableByNameAndPollIdQuery()
                        {
                            VariableName = variableDto.Name,
                            PollId = AsociatedPollId
                        };
                        var responseQuery = await _mediator.Send(query);
                        if (responseQuery.Success == true && responseQuery.Status == QueryEnums.QueryResultStatus.NotFound && !IsNewVersion)
                        {
                            VersionNumber += 1;
                            IsNewVersion = true;
                        }
                    }
                    CreateVariableCommand createVariableCommand = new CreateVariableCommand()
                    {
                        Variable = variableDto,
                        PollId = AsociatedPollId,
                        ComponentId = AsociatedComponentId
                    };
                    CreateCommandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);

                    if (createdVariable.Success && createdVariable.Entity != null)
                    {
                        int asociatedVariableId = createdVariable.Entity.Id;
                        CreateCommandResponse<Variable> createdPollVariable = await CreateRelationshipPollVariableAsync(variableDto, AsociatedPollId, asociatedVariableId);
                        createdVariable.Entity.PollVariableId = createdPollVariable.Entity!.PollVariableId;
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
        public async Task CreateAnswersAsync(PollDTO PollToCreate, List<Component> CreatedComponents, CreateCommandResponse<PollInstance> CreatedPollInstance)
        {
            var componentDict = CreatedComponents.ToDictionary(C => C.Name, C => C);

            List<AnswerDTO> answersToCreate = [];

            foreach (ComponentDTO component in PollToCreate.Components)
            {
                componentDict.TryGetValue(component.Name, out var componentByName);
                if (componentByName != null)
                {
                    var variableDict = componentByName.Variables.ToDictionary(V => V.Name, V => V);

                    foreach (VariableDTO variable in component.Variables)
                    {
                        try
                        {
                            if (variableDict.TryGetValue(variable.Name, out var variableByName))
                            {
                                AnswerDTO answerToCreate = variable.Answer != null ? variable.Answer : new AnswerDTO();
                                answerToCreate.PollVariableId = variableByName.PollVariableId;
                                answerToCreate.PollInstanceId = CreatedPollInstance.Entity!.Id;
                                answerToCreate.Audit = new AuditInfo()
                                {
                                    CreatedBy = "Cosmic latte import",
                                    CreatedAt = DateTime.UtcNow,
                                    ModifiedAt = DateTime.UtcNow,
                                };
                                answerToCreate.Version = new VersionInfo()
                                {
                                    VersionNumber = VersionNumber,
                                    VersionDate = InitDate
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
        public async Task<List<Component>> CreateComponentsAndVariablesAsync(ICollection<ComponentDTO> ComponentDtoList, int AsociatedPollId)
        {
            List<Component> createdComponents = [];
            foreach (ComponentDTO componentDto in ComponentDtoList)
            {
                try
                {
                    if (!IsNewPoll)
                    {
                        var componentOldQuery = new GetComponentByNameAndPollIdQuery()
                        {
                            ComponentName = componentDto.Name,
                            PollId = AsociatedPollId
                        };
                        var componentOld = await _mediator.Send(componentOldQuery);
                        if (componentOld.Status == QueryEnums.QueryResultStatus.NotFound && !IsNewVersion)
                        {
                            VersionNumber += 1;
                            IsNewVersion = true;
                        }
                    }
                    CreateCommandResponse<Component> createdComponent = await CreateComponentAsync(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.Entity!.Id;
                        List<Variable> createdVariables = await CreateVariablesAsync(componentDto.Variables, AsociatedPollId, asociatedComponentId);
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