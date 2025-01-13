using Entities;
using Microsoft.Extensions.Configuration;
using Services;

namespace Infrastructure.CosmicLatteClient.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService<CosmicLatteStatus>
    {
        private const string _PathEvalaution = "evaluations";
        private const string _HeaderApiKey = "x-apikey";
        private string _apiKey;
        private string _apiUrl;

        private readonly HttpClient _httpClient;

        public CosmicLatteAPIService(IConfiguration configuration, HttpClient httpClient)
        {
            _apiKey = configuration["CosmicLatte:ApiKey"];
            _apiUrl = configuration["CosmicLatte:URL"];
            _httpClient = httpClient;
        }
        public async Task<CosmicLatteStatus> CosmicApiIsHealthy()
        {
            string path = _apiUrl + _PathEvalaution;
            var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
            request.Headers.Add(_HeaderApiKey, _apiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return new CosmicLatteStatus(response.IsSuccessStatusCode ? true : false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}");
                throw new Exception($"There was an error with the request");
            }
        }
    }
}