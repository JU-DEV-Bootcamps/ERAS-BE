using ERAS.Application.Services;
using ERAS.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ERAS.Infrastructure.External.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService
    {
        private const string PathEvalaution = "evaluations";
        private const string HeaderApiKey = "x-apikey";
        private string _apiKey;
        private string _apiUrl;

        private readonly HttpClient _httpClient;

        public CosmicLatteAPIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _apiKey = configuration["CosmicLatte:ApiKey"];
            _apiUrl = configuration.GetSection("CosmicLatteUrl").Value;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
        }
        public async Task<CosmicLatteStatus> CosmicApiIsHealthy()
        {
            string path = _apiUrl + PathEvalaution;
            var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
            request.Headers.Add(HeaderApiKey, _apiKey);

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