using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore.Metadata;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy(string ApiKey, string ApiUrl);
        Task<List<PollDTO>> GetAllPollsPreview(string Name, string StartDate, string EndDate, string ApiKey, string ApiUrl);
        Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> PollsDtos);
        Task<List<PollDataItem>> GetPollsNameList(string BaseUrl, string ApiKey);
    }
}

