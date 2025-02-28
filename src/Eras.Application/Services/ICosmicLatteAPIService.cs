using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
        Task<List<PollDTO>> GetAllPollsPreview(string name, string startDate, string endDate);
        Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> pollsDtos);
        Task<List<PollDataItem>> GetPollsNameList();
    }
}

