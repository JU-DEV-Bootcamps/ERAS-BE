using Eras.Application.Dtos;
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
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Eras.Domain.Common;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Variable = Eras.Domain.Entities.Variable;
using Component = Eras.Domain.Entities.Component;
using Eras.Application.Features.StudentsDetails.Queries.GetStudentDetailByStudentId;
using Eras.Application.Models.Response.Common;
using Eras.Application.Features.Students.Queries.GetByEmail;
using Eras.Domain.Common.Exceptions;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Application.Models.Enums;
using Eras.Application.Features.PollVersions.Queries.GetAllByPoll;
using Eras.Application.Features.PollVersions.Commands.CreatePollVersion;
using Eras.Application.Features.Components.Queries.GetByName;
using Eras.Application.Features.Variables.Queries.GetByname;

namespace Eras.Application.Services
{
    public class PollOrchestratorService
    {
        private readonly IMediator _mediator;
        ILogger<PollOrchestratorService> _logger;
        private bool newPoll=false;
        private bool newComponent = false;
        private bool newVariable = false;

        public PollOrchestratorService(IMediator Mediator, ILogger<PollOrchestratorService> Logger )
        {
            _logger = Logger;
            _mediator = Mediator;
        }

        public async Task<CreateCommandResponse<CreatedPollDTO?>> ImportPollInstancesAsync(List<PollDTO> PollsToCreate)
        {
            try
            {
                var newDatePoll = DateTime.UtcNow;
                // Create poll
                PollDTO pollDTO = PollsToCreate[0];
                CreateCommandResponse<Poll> createdPollResponse = await CreatePollAsync(pollDTO);
                int createdPollsInstances = 0;
                CreatedPollDTO createdPoll = new CreatedPollDTO();

                if (createdPollResponse.Success)
                {
                    var pollToUse = createdPollResponse.Entity.ToDto();
                    pollToUse.PollVersions = pollDTO.PollVersions;
                    // Create components, variables and poll_variables (intermediate table)
                    List<Component> createdComponents = await CreateComponentsAndVariablesAsync(PollsToCreate[0].Components, 
                        createdPollResponse.Entity.Id);

                    foreach (PollDTO pollToCreate in PollsToCreate)
                    {
                        // Create students
                        CreateCommandResponse<Student> createdStudent = await CreateStudentFromPollAsync(pollToCreate);
                        if (createdStudent.Success)
                        {
                            createdPoll.studentDTOs.Add(createdStudent.Entity.ToDto());
                            // Create poll instances
                            CreateCommandResponse<PollInstance?> createdPollInstance = await CreatePollInstanceAsync(createdStudent.Entity, 
                                createdPollResponse.Entity.Uuid, pollToCreate.FinishedAt);
                            // Create asnswers
                            if (createdPollInstance.Success)
                            {
                                await CreateAnswersAsync(pollToCreate, createdComponents, createdPollInstance);
                            }
                            createdPollsInstances++;
                        }
                    }
                    if (newPoll || (!newPoll && (newComponent || newVariable)))
                    {
                        pollToUse.PollVersions = new List<PollVersionDTO>
                        {
                            new PollVersionDTO()
                            {
                                Name = "Version"+newDatePoll,
                                Date = newDatePoll,
                            }
                        };
                        await CreateAndValidatePollVersionAsync(pollToUse);
                    }
                }
                return new CreateCommandResponse<CreatedPollDTO?>(createdPoll, createdPollsInstances, "Success", true);
            }
            catch (Exception ex)
            {
                return new CreateCommandResponse<CreatedPollDTO?>(null, 0, $"Error during import process {ex.Message}", false);
            }
        }
        public async Task<CreateCommandResponse<PollInstance?>> CreatePollInstanceAsync(Student Student, string PollUuid, DateTime FinishedAt)
        {
            try
            {
                PollInstance pollInstance = new PollInstance() { Uuid = PollUuid, Student = Student, FinishedAt = FinishedAt};

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
                return new CreateCommandResponse<PollInstance?>(null, 0, "Error", false);
            }
        }
        public async Task<CreateCommandResponse<StudentDetail>> CreateStudentDetailAsync(int StudentId)
        {
            GetStudentDetailByStudentIdQuery query = new GetStudentDetailByStudentIdQuery()
            {
                StudentId = StudentId
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
        public async Task<CreateCommandResponse<Cohort>> CreateAndSetStudentCohortAsync(StudentDTO StudentDto,CohortDTO Cohort)
        {
            Cohort.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateCohortCommand createCohortCommand = new CreateCohortCommand() { CohortDto = Cohort };
            CreateCommandResponse <Cohort> createdCohort = await _mediator.Send(createCohortCommand);

            if (createdCohort.Success)
            {
                CreateStudentCohortCommand createStudentCohortCommand = new CreateStudentCohortCommand() { CohortId = createdCohort.Entity.Id, StudentId = StudentDto.Id };
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
                if (createdStudent.Success)
                {
                    CreateCommandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetailAsync(createdStudent.Entity.Id);
                    CohortDTO cohortToCreate = studentToCreate.Cohort;
                    createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                    CreateCommandResponse<Cohort> createdCohort = await CreateAndSetStudentCohortAsync(createdStudent.Entity.ToDto(), cohortToCreate);

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
                    newPoll = true;
                    CreatePollCommand createPollCommand = new CreatePollCommand() { Poll = PollToCreate };
                    return await _mediator.Send(createPollCommand);
                }
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
                List <Variable> createdVariables = new List <Variable>();
                if (VariablesDtos == null) return createdVariables;

                foreach (VariableDTO variableDto in VariablesDtos)
                {
                    variableDto.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };
                    var query = new GetVariableByNameQuery() { VariableName = variableDto.Name };
                    var responseQuery = await _mediator.Send(query);
                    if (responseQuery.Success == true && responseQuery.Status == QueryEnums.QueryResultStatus.NotFound)
                        newVariable = true;
                    CreateVariableCommand createVariableCommand = new CreateVariableCommand()
                    {
                        Variable = variableDto,
                        PollId = AsociatedPollId,
                        ComponentId = AsociatedComponentId
                    };
                    CreateCommandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);

                    if (createdVariable.Success)
                    {
                        int asociatedVariableId = createdVariable.Entity.Id;
                        CreateCommandResponse<Variable> createdPollVariable = await CreateRelationshipPollVariableAsync(variableDto, AsociatedPollId, asociatedVariableId);
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
                                answerToCreate.PollInstanceId = CreatedPollInstance.Entity.Id;
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
                    var componentOldQuery = new GetComponentByNameQuery() { componentName = componentDto.Name };
                    var componentOld = await _mediator.Send(componentOldQuery);
                    if (componentOld.Status == QueryEnums.QueryResultStatus.NotFound) {
                        newComponent = true;
                    }
                    CreateCommandResponse<Component> createdComponent = await CreateComponentAsync(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.Entity.Id;
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
        public async Task<CreateCommandResponse<PollVersion>> CreateAndValidatePollVersionAsync(PollDTO Poll)
        {
            var newVersionDate = new DateTime();
            if (Poll.PollVersions.Count() > 0)
                newVersionDate = Poll.PollVersions.Last().Date;
            var allPollVersionsQuery = new GetAllPollVersionByPollQuery() { PollId = Poll.Id };
            var allPollVersionsResponse = await _mediator.Send(allPollVersionsQuery);
            var oldDate = new DateTime();
            if (allPollVersionsResponse.Body.Count() > 0)
                oldDate = allPollVersionsResponse.Body[0].Date;
            if (newVersionDate <= oldDate)
            {
                return new CreateCommandResponse<PollVersion>
                (new PollVersion(), "No new version detected", true,
                CommandEnums.CommandResultStatus.NotFound);
            }
            var newPollVersion = Poll.PollVersions.Last();
            newPollVersion.Audit = new AuditInfo()
            {
                CreatedBy = "CL Import",
                CreatedAt = DateTime.UtcNow
            };
            var newVersionCommand = new CreatePollVersionCommand()
            {
                PollVersionDTO = newPollVersion
            };
            newVersionCommand.PollId = Poll.Id;
            var createResponse = await _mediator.Send(newVersionCommand);
            return createResponse;
        }

    }
}
