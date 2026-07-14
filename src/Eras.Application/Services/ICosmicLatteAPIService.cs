using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy(string ApiKey, string ApiUrl);
        Task<List<PollDTO>> GetAllPollsPreview(string Name, string StartDate, string EndDate, string ApiKey, string ApiUrl);

        /// <summary>
        /// Extracts respondents one at a time from Cosmic Latte (parallel HTTP, serialized callback),
        /// invoking <paramref name="OnExtracted"/> per respondent with its PollDTO and whether the
        /// student was already imported. Enables progress reporting during the extraction phase.
        /// </summary>
        Task ExtractRespondentsAsync(string EvaluationSetName, string StartDate, string EndDate, string ApiKey, string ApiUrl, Func<PollDTO, bool, Task> OnExtracted);
        Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> PollsDtos, int EvaluationId);
        Task<List<PollDataItem>> GetPollsNameList(string BaseUrl, string ApiKey);
    }
}

