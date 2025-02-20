using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Answers.Commands.CreateAnswer;
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
using System.Diagnostics;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;

namespace Eras.Application.Services
{
    public class PollOrchestratorService // Create interface or comand
    {
        private readonly IMediator _mediator;
        ILogger<PollOrchestratorService> _logger;
        private System.Diagnostics.Stopwatch _stopwatch;

        public PollOrchestratorService(IMediator mediator, 
            ILogger<PollOrchestratorService> logger
            )
        {
            _logger = logger;
            _mediator = mediator;
            _stopwatch = Stopwatch.StartNew();
        }

        public async Task<CreateComandResponse<Poll>> ImportPollInstances(List<PollDTO> pollsToCreate)
        {
            try
            {
                _logger.LogError($"------------- ImportPollInstances:  INICIA EN :{_stopwatch.ElapsedMilliseconds} ms");
                // Create poll
                PollDTO pollDTO = pollsToCreate[0];
                CreateComandResponse<Poll> createdPollResponse = CreatePoll(pollDTO).Result;
                if (createdPollResponse.Entity == null) return createdPollResponse;

                // Create components, variables and poll_variables (intermediate table)
                List<Component> createdComponents = await CreateComponentsAndVariables(pollsToCreate[0].Components, createdPollResponse.Entity.Id);

                int createdPollsInstances = 0;
                foreach (PollDTO pollToCreate in pollsToCreate)
                {
                    // Create students
                    CreateComandResponse<Student> createdStudent = await CreateStudentFromPoll(pollToCreate);

                    // Create poll instances
                    CreateComandResponse<PollInstance> createdPollInstance = await CreatePollInstance(createdStudent.Entity, createdPollResponse.Entity.Uuid, pollToCreate.FinishedAt);

                    // Create asnswers
                    if (createdPollInstance.Success)
                    {
                        await CreateAnswers(pollToCreate, createdComponents, createdPollInstance);
                    }
                    createdPollsInstances++;
                }
                _logger.LogError($"------------- ImportPollInstances:  FIN en :{_stopwatch.ElapsedMilliseconds} ms");
                _stopwatch.Stop();
                return new CreateComandResponse<Poll>(null, createdPollsInstances, "Success", true);
            }
            catch (Exception ex)
            {
                return new CreateComandResponse<Poll>(null, 0, $"Error during import process {ex.Message}", false);
            }
        }
        public async Task<CreateComandResponse<PollInstance>> CreatePollInstance(Student student, string pollUuid, DateTime finishedAt)
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
                return new CreateComandResponse<PollInstance>(null, 0, "Error", false);
            }
        }
        public async Task<CreateComandResponse<StudentDetail>> CreateStudentDetail(int studentId)
        {
            StudentDetailDTO studentDetailDTO = new StudentDetailDTO() { StudentId = studentId  };
            studentDetailDTO.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateStudentDetailCommand createStudentDetailCommand = new CreateStudentDetailCommand() { StudentDetailDto = studentDetailDTO };
            return await _mediator.Send(createStudentDetailCommand);
        }
        public async Task<CreateComandResponse<Cohort>> CreateAndSetStudentCohort(StudentDTO studentDto,CohortDTO cohort)
        {
            cohort.Audit = new AuditInfo()
            {
                CreatedBy = "Cosmic latte import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            CreateCohortCommand createCohortCommand = new CreateCohortCommand() { CohortDto = cohort };
            CreateComandResponse <Cohort> createdCohort = await _mediator.Send(createCohortCommand);

            if (createdCohort.Success)
            {
                CreateStudentCohortCommand createStudentCohortCommand = new CreateStudentCohortCommand() { CohortId = createdCohort.Entity.Id, StudentId = studentDto.Id };
                CreateComandResponse<Student> createdStudentCohort = await _mediator.Send(createStudentCohortCommand);
            }

            return createdCohort;
        }
        public async Task<CreateComandResponse<Student>> CreateStudentFromPoll(PollDTO pollToCreate)
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
                CreateComandResponse<Student> createdStudent = await _mediator.Send(createStudentCommand); 

                if (createdStudent.Success)
                {
                    CreateComandResponse<StudentDetail> createdStudentDetail = await CreateStudentDetail(createdStudent.Entity.Id);
                    CohortDTO cohortToCreate = studentToCreate.Cohort;
                    createdStudent.Entity.StudentDetail = createdStudentDetail.Entity;
                    CreateComandResponse<Cohort> createdCohort = await CreateAndSetStudentCohort(createdStudent.Entity.ToDto(), cohortToCreate);
                    createdStudent.Entity.Cohort = createdCohort.Entity; 
                }
                return createdStudent;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating student: {ex.Message}");
                return new CreateComandResponse<Student>(null,0, "Error", false);
            }
        }
        public async Task<CreateComandResponse<Poll>> CreatePoll(PollDTO pollToCreate)
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
                return new CreateComandResponse<Poll>(null, 0, "Error", false);
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
                return new CreateComandResponse<Variable>(null, 0, "Error", false);
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
                    CreateComandResponse<Variable> createdVariable = await _mediator.Send(createVariableCommand);

                    if (createdVariable.Success)
                    {   // Add manual relationship between poll_variable
                        int asociatedVariableId = createdVariable.Entity.Id;
                        CreateComandResponse<Variable> createdPollVariable = await CreateRelationshipPollVariable(variableDto, asociatedPollId, asociatedVariableId);
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
        
        public async Task CreateAnswers(PollDTO pollToCreate, List<Component> createdComponents, CreateComandResponse<PollInstance> createdPollInstance)
        {
            // Answer needs to associate the poll_variable id, from the intermediate table that relates poll and variables
            // since there is no direct relationship, it must iterate again over the elements already created   
            // This way, the information between the id that corresponds to each poll_variable can be crossed with the corresponding answer
            // This is weird, surely there is a more efficient way to do this


            var componentDict = createdComponents.ToDictionary(c => c.Name, c => c);

            List<AnswerDTO> answersToCreate = [];

            foreach (ComponentDTO component in pollToCreate.Components)
            {
                componentDict.TryGetValue(component.Name, out var componentByName);  // Component? componentByName = createdComponents.FirstOrDefault(c => c.Name == component.Name);
                if (componentByName != null)
                {
                    var variableDict = componentByName.Variables.ToDictionary(v => v.Name, v => v); // Variable ? variableByName = componentByName.Variables.FirstOrDefault(v => v.Name == variable.Name);

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

                                // CreateAnswerCommand createAnswerCommand = new CreateAnswerCommand() { Answer = answerToCreate };                                
                                // await _mediator.Send(createAnswerCommand); // await _mediator.Send(createAnswerCommand);
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
        /*
        public async Task CreateAnswers(PollDTO pollToCreate, List<Component> createdComponents, CreateComandResponse<PollInstance> createdPollInstance)
        {
            if (createdPollInstance?.Entity == null)
            {
                _logger.LogError("Poll instance is null, cannot create answers.");
                return;
            }

            // Crear diccionarios para acceso rápido
            var componentDict = createdComponents.ToDictionary(c => c.Name, c => c);

            List<Task> tasks = [];

            foreach (var component in pollToCreate.Components)
            {
                if (!componentDict.TryGetValue(component.Name, out var componentByName))
                {
                    _logger.LogWarning($"Component {component.Name} not found in created components.");
                    continue;
                }

                // Convertir variables en diccionario dentro de cada componente
                var variableDict = componentByName.Variables.ToDictionary(v => v.Name, v => v);

                foreach (var variable in component.Variables)
                {
                    if (!variableDict.TryGetValue(variable.Name, out var variableByName))
                    {
                        _logger.LogWarning($"Variable {variable.Name} not found in component {component.Name}.");
                        continue;
                    }

                    AnswerDTO answerToCreate = variable.Answer;
                    answerToCreate.PollInstanceId = createdPollInstance.Entity.Id;
                    answerToCreate.PollVariableId = variableByName.PollVariableId;
                    answerToCreate.Audit = new AuditInfo()
                    {
                        CreatedBy = "Cosmic latte import",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow,
                    };

                    CreateAnswerCommand createAnswerCommand = new() { Answer = answerToCreate };
                    tasks.Add(_mediator.Send(createAnswerCommand));
                }
            }

            // Ejecutar todas las tareas en paralelo
            await Task.WhenAll(tasks);
        }
        */
        /*
        public async Task<List<Component>> CreateComponentsAndVariables(ICollection<ComponentDTO> componentDtoList, int asociatedPollId)
        {
            List<Task<Component>> componentTasks = [];

            foreach (var componentDto in componentDtoList)
            {
                componentTasks.Add(CreateComponentWithVariables(componentDto, asociatedPollId));
            }

            Component[] createdComponents = await Task.WhenAll(componentTasks);
            return createdComponents.Where(c => c != null).ToList();
        } 
        */
        /*
        private async Task<Component?> CreateComponentWithVariables(ComponentDTO componentDto, int asociatedPollId)
        {
            try
            {
                CreateComandResponse<Component> createdComponent = await CreateComponent(componentDto);
                if (createdComponent.Success)
                {
                    int asociatedComponentId = createdComponent.Entity.Id;
                    var createdVariables = await CreateVariables(componentDto.Variables, asociatedPollId, asociatedComponentId);
                    createdComponent.Entity.Variables = createdVariables;
                    return createdComponent.Entity;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating component: {ex.Message}");
            }

            return null;
        }
        */
        public async Task<CreateComandResponse<Component>> CreateComponent(ComponentDTO componentDto)
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
                return new CreateComandResponse<Component>(null, 0, "Error", false);
            }
        } 
        public async Task<List<Component>> CreateComponentsAndVariables(ICollection<ComponentDTO> componentDtoList, int asociatedPollId)
        {

            List<Component> createdComponents = [];

            foreach (ComponentDTO componentDto in componentDtoList)
            {
                try
                {
                    CreateComandResponse<Component> createdComponent = await CreateComponent(componentDto);
                    if (createdComponent.Success)
                    {
                        int asociatedComponentId = createdComponent.Entity.Id;
                        List<Variable> createdVariables =  await CreateVariables(componentDto.Variables, asociatedPollId, asociatedComponentId);
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