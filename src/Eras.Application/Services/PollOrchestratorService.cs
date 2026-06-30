using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Polls.Commands.UpdatePoll;
using Eras.Application.Mappers;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

using Component = Eras.Domain.Entities.Component;

namespace Eras.Application.Services
{
    /// <summary>
    /// Coordinates the Cosmic Latte import: wraps the whole operation in a single transaction and
    /// delegates the work to focused collaborators (poll structure, students, instances + answers).
    /// </summary>
    public class PollOrchestratorService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PollOrchestratorService> _logger;
        private readonly IEvaluationRepository _evaluationRepository;
        private readonly IPollInstanceRepository _pollInstanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PollStructureImporter _pollStructureImporter;
        private readonly StudentImporter _studentImporter;
        private readonly PollInstanceImporter _pollInstanceImporter;

        public PollOrchestratorService(
            IMediator Mediator,
            ILogger<PollOrchestratorService> Logger,
            IEvaluationRepository EvaluationRepository,
            IPollInstanceRepository PollInstanceRepository,
            IUnitOfWork UnitOfWork,
            PollStructureImporter PollStructureImporter,
            StudentImporter StudentImporter,
            PollInstanceImporter PollInstanceImporter)
        {
            _logger = Logger;
            _mediator = Mediator;
            _evaluationRepository = EvaluationRepository;
            _pollInstanceRepository = PollInstanceRepository;
            _unitOfWork = UnitOfWork;
            _pollStructureImporter = PollStructureImporter;
            _studentImporter = StudentImporter;
            _pollInstanceImporter = PollInstanceImporter;
        }

        public async Task<CreateCommandResponse<CreatedPollDTO>> ImportPollInstancesAsync(List<PollDTO> PollsToCreate, int EvaluationId)
        {
            try
            {
                // Wrap the whole import in a single transaction: poll, components, variables,
                // poll instances and answers either all persist or none do (no orphaned data).
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var context = new ImportContext(DateTime.UtcNow);
                    PollDTO pollDTO = PollsToCreate[0];
                    CreateCommandResponse<Poll> createdPollResponse = await _pollStructureImporter.CreatePollAsync(pollDTO, context);
                    if (!createdPollResponse.Success || createdPollResponse.Entity == null)
                    {
                        throw new InvalidOperationException($"Import aborted: could not resolve poll '{pollDTO.Name}'.");
                    }

                    int createdPollsInstances = 0;
                    CreatedPollDTO createdPoll = new CreatedPollDTO();
                    var pollToUse = createdPollResponse.Entity.ToDto();

                    await MarkEvaluationReadyAsync(EvaluationId, pollToUse.Id);

                    // Create components, variables and poll_variables (intermediate table)
                    List<Component> createdComponents = await _pollStructureImporter.CreateComponentsAndVariablesAsync(
                        PollsToCreate[0].Components, createdPollResponse.Entity.Id, context);

                    if (context.IsNewVersion)
                    {
                        pollToUse.LastVersion = context.VersionNumber;
                        pollToUse.LastVersionDate = context.InitDate;
                        await _mediator.Send(new UpdatePollByIdCommand() { PollDTO = pollToUse });
                    }

                    foreach (PollDTO pollToCreate in PollsToCreate)
                    {
                        CreateCommandResponse<Student> createdStudent = await _studentImporter.CreateStudentFromPollAsync(pollToCreate);
                        if (!createdStudent.Success || createdStudent.Entity == null) continue;

                        createdPoll.studentDTOs.Add(createdStudent.Entity.ToDto());

                        // All entries in PollsToCreate share the same poll template (resolved once
                        // above), so reuse it instead of re-querying per instance. Persist the answers
                        // hash so future imports can match duplicates via index.
                        string answersHash = _pollInstanceRepository.ComputeAnswersHash(pollToCreate);
                        CreateCommandResponse<PollInstance> createdPollInstance = await _pollInstanceImporter.CreatePollInstanceAsync(
                            createdStudent.Entity, createdPollResponse.Entity.Uuid, pollToCreate.FinishedAt, EvaluationId, context, answersHash);

                        if (!createdPollInstance.Success) continue;

                        PollInstance? sourceInstance = await _pollInstanceRepository.FindMatchingSourceInstanceAsync(
                            studentId: createdStudent.Entity.Id,
                            currentPollInstanceId: createdPollInstance.Entity.Id,
                            incomingPoll: pollToCreate);

                        if (sourceInstance != null)
                        {
                            // Mark source instance, no new answers created
                            await _pollInstanceRepository.SetSourceInstanceAsync(createdPollInstance.Entity.Id, sourceInstance.Id);
                        }
                        else
                        {
                            await _pollInstanceImporter.CreateAnswersAsync(pollToCreate, createdComponents, createdPollInstance, context);
                        }
                        await _mediator.Publish(new AnswerSubmittedEvent(EvaluationId));
                        createdPollsInstances++;
                    }

                    return new CreateCommandResponse<CreatedPollDTO>(createdPoll, createdPollsInstances, "Success", true);
                });
            }
            catch (Exception ex)
            {
                // The transaction has already rolled back; surface the failure to the caller.
                _logger.LogError(ex, "Error during import process; transaction rolled back");
                return new CreateCommandResponse<CreatedPollDTO>(null, 0, $"Error during import process {ex.Message}", false);
            }
        }

        private async Task MarkEvaluationReadyAsync(int EvaluationId, int PollId)
        {
            var evaluation = await _evaluationRepository.GetStatusById(EvaluationId);
            if (evaluation == null || !evaluation.Status.Equals(EvaluationConstants.EvaluationStatus.Pending.ToString()))
            {
                return;
            }

            evaluation.Status = EvaluationConstants.EvaluationStatus.Ready.ToString();
            await _evaluationRepository.UpdateAsync(evaluation);

            var evaluationDto = evaluation.ToDto();
            evaluationDto.PollId = PollId;
            await _mediator.Send(new CreateEvaluationPollCommand() { EvaluationDTO = evaluationDto });
        }
    }
}
