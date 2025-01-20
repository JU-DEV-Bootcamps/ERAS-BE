using Eras.Domain.Entities;

namespace Eras.Application.Services
{
    public interface ICosmicLatteAPIService
    {
        Task<CosmicLatteStatus> CosmicApiIsHealthy();
    }
}

