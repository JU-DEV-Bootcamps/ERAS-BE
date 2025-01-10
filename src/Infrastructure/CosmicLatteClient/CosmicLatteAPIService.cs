using Entities;
using Microsoft.Extensions.Configuration;
using Services;

namespace Infrastructure.CosmicLatteClient.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService<CosmicLatteStatus>
    {
        private const string _PATH_EVALUATION = "evaluations";
        private const string _HEADER_API_KEY = "x-apikey";
        private string _API_KEY;
        private string _API_URL;

        private readonly HttpClient _httpClient;

        public CosmicLatteAPIService(IConfiguration configuration, HttpClient httpClient)
        {
            _API_KEY = configuration["CosmicLatte:ApiKey"];
            _API_URL = configuration["CosmicLatte:URL"];
            _httpClient = httpClient;
        }
        public async Task<CosmicLatteStatus> CosmicApiIsHealthy()
        {
            string path = _API_URL + _PATH_EVALUATION;
            var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
            request.Headers.Add(_HEADER_API_KEY, _API_KEY);

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