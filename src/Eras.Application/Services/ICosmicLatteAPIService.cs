using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
        Task<List<PollDTO>> GetAllPollsPreview(string Name, string StartDate, string EndDate);
        Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> PollsDtos);
        Task<List<PollDataItem>> GetPollsNameList();
    }
}

