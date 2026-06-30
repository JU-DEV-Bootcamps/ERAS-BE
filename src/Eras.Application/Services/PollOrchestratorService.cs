using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Polls.Commands.UpdatePoll;
using Eras.Application.Features.Polls.Queries.GetPollByName;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.Extensions.Logging;

using Component = Eras.Domain.Entities.Component;
using Variable = Eras.Domain.Entities.Variable;

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
        private readonly IVariableRepository _variableRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PollStructureImporter _pollStructureImporter;
        private readonly StudentImporter _studentImporter;
        private readonly PollInstanceImporter _pollInstanceImporter;

        public PollOrchestratorService(
            IMediator Mediator,
            ILogger<PollOrchestratorService> Logger,
            IEvaluationRepository EvaluationRepository,
            IPollInstanceRepository PollInstanceRepository,
            IVariableRepository VariableRepository,
            IUnitOfWork UnitOfWork,
            PollStructureImporter PollStructureImporter,
            StudentImporter StudentImporter,
            PollInstanceImporter PollInstanceImporter)
        {
            _logger = Logger;
            _mediator = Mediator;
            _evaluationRepository = EvaluationRepository;
            _pollInstanceRepository = PollInstanceRepository;
            _variableRepository = VariableRepository;
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

        /// <summary>
        /// Phase 1 of an async import: resolve/create the poll template (poll, components, variables,
        /// poll_variables) and mark the evaluation Ready. Runs in its own transaction. Students are
        /// processed separately via <see cref="ProcessStudentAsync"/>.
        /// </summary>
        public async Task<CreateCommandResponse<Poll>> SetupImportStructureAsync(List<PollDTO> Polls, int EvaluationId)
        {
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var context = new ImportContext(DateTime.UtcNow);
                    PollDTO pollDTO = Polls[0];
                    CreateCommandResponse<Poll> createdPollResponse = await _pollStructureImporter.CreatePollAsync(pollDTO, context);
                    if (!createdPollResponse.Success || createdPollResponse.Entity == null)
                    {
                        throw new InvalidOperationException($"Import setup aborted: could not resolve poll '{pollDTO.Name}'.");
                    }

                    var pollToUse = createdPollResponse.Entity.ToDto();
                    await MarkEvaluationReadyAsync(EvaluationId, pollToUse.Id);

                    await _pollStructureImporter.CreateComponentsAndVariablesAsync(
                        pollDTO.Components, createdPollResponse.Entity.Id, context);

                    if (context.IsNewVersion)
                    {
                        pollToUse.LastVersion = context.VersionNumber;
                        pollToUse.LastVersionDate = context.InitDate;
                        await _mediator.Send(new UpdatePollByIdCommand() { PollDTO = pollToUse });
                    }

                    return new CreateCommandResponse<Poll>(createdPollResponse.Entity, 0, "Success", true);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during import setup; transaction rolled back");
                return new CreateCommandResponse<Poll>(null, 0, $"Error during import setup: {ex.Message}", false);
            }
        }

        /// <summary>
        /// Phase 2 of an async import: process a single student (student + poll instance + answers)
        /// in its own transaction. The poll structure must already exist (see
        /// <see cref="SetupImportStructureAsync"/>); it is rebuilt from the database so retries do not
        /// depend on in-memory state. A failure here only rolls back this student's work.
        /// </summary>
        public async Task<ImportStudentResult> ProcessStudentAsync(PollDTO PollToCreate, int EvaluationId)
        {
            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var pollResponse = await _mediator.Send(new GetPollByNameQuery() { pollName = PollToCreate.Name });
                    if (!pollResponse.Success || pollResponse.Body == null
                        || pollResponse.Status == QueryEnums.QueryResultStatus.NotFound)
                    {
                        throw new InvalidOperationException($"Poll '{PollToCreate.Name}' not found; run import setup first.");
                    }
                    Poll poll = pollResponse.Body;

                    var context = new ImportContext(DateTime.UtcNow)
                    {
                        IsNewPoll = false,
                        IsNewVersion = false,
                        VersionNumber = poll.LastVersion,
                    };

                    List<Component> createdComponents = await RebuildPollStructureAsync(poll, PollToCreate);

                    CreateCommandResponse<Student> createdStudent = await _studentImporter.CreateStudentFromPollAsync(PollToCreate);
                    if (!createdStudent.Success || createdStudent.Entity == null)
                    {
                        throw new InvalidOperationException("Could not create or resolve the student for this submission.");
                    }

                    string answersHash = _pollInstanceRepository.ComputeAnswersHash(PollToCreate);
                    CreateCommandResponse<PollInstance> createdPollInstance = await _pollInstanceImporter.CreatePollInstanceAsync(
                        createdStudent.Entity, poll.Uuid, PollToCreate.FinishedAt, EvaluationId, context, answersHash);
                    if (!createdPollInstance.Success || createdPollInstance.Entity == null)
                    {
                        throw new InvalidOperationException("Could not create the poll instance for this submission.");
                    }

                    PollInstance? sourceInstance = await _pollInstanceRepository.FindMatchingSourceInstanceAsync(
                        studentId: createdStudent.Entity.Id,
                        currentPollInstanceId: createdPollInstance.Entity.Id,
                        incomingPoll: PollToCreate);

                    if (sourceInstance != null)
                    {
                        await _pollInstanceRepository.SetSourceInstanceAsync(createdPollInstance.Entity.Id, sourceInstance.Id);
                    }
                    else
                    {
                        await _pollInstanceImporter.CreateAnswersAsync(PollToCreate, createdComponents, createdPollInstance, context);
                    }
                    await _mediator.Publish(new AnswerSubmittedEvent(EvaluationId));
                });
                return new ImportStudentResult(true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing student import; transaction rolled back");
                return new ImportStudentResult(false, ex.Message);
            }
        }

        /// <summary>
        /// Rebuilds the in-memory component/variable structure (with PollVariableId) from the database
        /// for the poll being imported, so <c>CreateAnswersAsync</c> can map answers without relying on
        /// the components created during setup.
        /// </summary>
        private async Task<List<Component>> RebuildPollStructureAsync(Poll Poll, PollDTO PollToCreate)
        {
            List<string> componentNames = PollToCreate.Components.Select(C => C.Name).Distinct().ToList();
            List<Variable> variables = await _variableRepository.GetAllByPollUuidAsync(Poll.Uuid, componentNames, true);
            return variables
                .GroupBy(V => V.ComponentName)
                .Select(Group => new Component { Name = Group.Key, Variables = Group.ToList() })
                .ToList();
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
