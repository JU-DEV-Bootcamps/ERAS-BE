using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Infrastructure;

public interface IPollOrchestratorServiceV2
{
    Task<CreateCommandResponse<CreatedPollDTO>> ImportPollInstancesAsync(List<PollDTO> PollsToCreate, int EvaluationId);
    Task<CreateCommandResponse<Poll>> SetupImportStructureAsync(List<PollDTO> Polls, int EvaluationId);
    Task<ImportStudentResult> ProcessStudentAsync(PollDTO PollToCreate, int EvaluationId);
}
