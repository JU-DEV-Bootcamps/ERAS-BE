namespace Domain.Services
{
    public interface ICosmicLatteAPIService<T>
    {
        Task<T> CosmicApiIsHealthy();
    }
}

