using Eras.Application.Dtos;
using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
        Task<int> ImportAllPolls(string name, string startDate, string endDate);

        // This should be used only for preview feat, now we are getting and saving data in one step
        // Task<string> GetPollById(string id); 
        // Task<List<string>> GetPolls(string name, string startDate, string endDate);
    }
}

