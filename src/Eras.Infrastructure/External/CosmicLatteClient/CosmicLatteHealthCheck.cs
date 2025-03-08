using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.External.CosmicLatteClient
{
    [ExcludeFromCodeCoverage]
    public class CosmicLatteHealthCheck : IHealthCheck
    {
        private const string _PATH_EVALUATION = "evaluations";
        private const string _HEADER_API_KEY = "x-apikey";
        private string _API_KEY;
        private string _API_URL;
        private readonly HttpClient _httpClient;

        public CosmicLatteHealthCheck(IConfiguration configuration, HttpClient httpClient)
        {
            _API_KEY = configuration.GetSection("CosmicLatte:ApiKey").Value ?? throw new Exception("Cosmic latte api key not found"); // this should be move to .env
            _API_URL = configuration.GetSection("CosmicLatte:BaseUrl").Value ?? throw new Exception("Cosmic latte Url not found"); // this should be move to .env
            _httpClient = httpClient;

        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            string path = _API_URL + _PATH_EVALUATION;
            var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
            request.Headers.Add(_HEADER_API_KEY, _API_KEY);

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await _httpClient.SendAsync(request);
                stopwatch.Stop();

                var data = new Dictionary<string, object>
                    {
                        { "date", DateTime.UtcNow.ToLocalTime().ToString("dd-MM-yyyy HH:mm") + "hs" },
                        { "ResponseTime", stopwatch.ElapsedMilliseconds },
                        { "StatusCode", response.StatusCode },
                        { "ContentLength", response.Content.Headers.ContentLength ?? 0 }
                    };

                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Cosmic Latte service is available", data);
                }
                else
                {
                    return HealthCheckResult.Unhealthy("Cosmic Latte service returned an unsuccessful status code", null, data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}");
                throw new Exception($"There was an error with the request");
            }
        }
    }
}
