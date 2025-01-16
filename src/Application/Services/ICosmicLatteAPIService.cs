using ERAS.Domain.Entities;

namespace ERAS.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
    }
}

