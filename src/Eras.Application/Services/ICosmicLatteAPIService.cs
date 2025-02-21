using Eras.Application.Dtos;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
        Task<List<PollDTO>> ImportAllPolls(string name, string startDate, string endDate);
        Task<List<PollDataItem>> GetPollsNameList();

        // This should be used only for preview feat, now we are getting and saving data in one step
        // Task<string> GetPollById(string id); 
        // Task<List<string>> GetPolls(string name, string startDate, string endDate);
    }
}

