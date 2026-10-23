using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

using Component = Eras.Domain.Entities.Component;

namespace Eras.Application.Services
{
    /// <summary>
    /// Imports a single student's poll instance and its answers.
    /// </summary>
    public class PollInstanceImporter
    {
        private readonly IMediator _mediator;
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly ILogger<PollInstanceImporter> _logger;

        public PollInstanceImporter(IMediator Mediator, IPollInstanceRepository PollInstanceRepository, ILogger<PollInstanceImporter> Logger)
        {
            _mediator = Mediator;
            _pollInstanceRepository = PollInstanceRepository;
            _logger = Logger;
        }

        public async Task<CreateCommandResponse<PollInstance>> CreatePollInstanceAsync(Student Student, string PollUuid, DateTime FinishedAt, int EvaluationId, ImportContext Context, string? AnswersHash = null)
        {
            try
            {
                bool alreadyExists = await _pollInstanceRepository.ExistsForStudentAndEvaluationAsync(Student.Id, PollUuid, EvaluationId);

                if (alreadyExists)
                {
                    var queryPollInstance = new GetPollInstanceByUuidAndStudentIdQuery()
                    {
                        PollUuid = PollUuid,
                        StudentId = Student.Id,
                        EvaluationId = EvaluationId
                    };
                    var responseQuery = await _mediator.Send(queryPollInstance);
                    return new CreateCommandResponse<PollInstance>(
                        responseQuery.Body, 0, "Already exists for this evaluation", true);
                }
                PollInstance pollInstance = new PollInstance()
                {
                    Uuid = PollUuid,
                    Student = Student,
                    FinishedAt = FinishedAt,
                    EvaluationId = EvaluationId,
                };

                pollInstance.Audit = new AuditInfo()
                {
                    CreatedBy = "Cosmic latte import",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };
                pollInstance.LastVersion = Context.VersionNumber;
                pollInstance.LastVersionDate = Context.InitDate;
                pollInstance.AnswersHash = AnswersHash;

                CreatePollInstanceCommand createPollInstanceCommand = new CreatePollInstanceCommand()
                {
                    PollInstance = pollInstance.ToDTO()
                };
                return await _mediator.Send(createPollInstanceCommand);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating poll instance: {ex.Message}");
                return new CreateCommandResponse<PollInstance>(new PollInstance(), 0, "Error", false, CommandEnums.CommandResultStatus.Error);
            }
        }

        public async Task CreateAnswersAsync(PollDTO PollToCreate, List<Component> CreatedComponents, CreateCommandResponse<PollInstance> CreatedPollInstance, ImportContext Context)
        {
            var componentDict = CreatedComponents.ToDictionary(C => C.Name, C => C);

            List<AnswerDTO> answersToCreate = [];

            foreach (ComponentDTO component in PollToCreate.Components)
            {
                componentDict.TryGetValue(component.Name, out var componentByName);
                if (componentByName != null)
                {
                    var variableDict = componentByName.Variables.ToDictionary(V => $"{V.Name}_{V.Position}", V => V);

                    foreach (VariableDTO variable in component.Variables)
                    {
                        try
                        {
                            string uniqueKey = $"{variable.Name}_{variable.Position}";
                            if (variableDict.TryGetValue(uniqueKey, out var variableByName))
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
                                    VersionNumber = Context.VersionNumber,
                                    VersionDate = Context.InitDate
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
    }
}
