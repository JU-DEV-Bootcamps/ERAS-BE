using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Infrastructure.External.CosmicLatteClient
{
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
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy();
                }
                else
                {
                    return HealthCheckResult.Unhealthy();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error : {e.Message}");
                throw new Exception($"There was an error with the request");
            }

            /*        
            var fullUrl = new Uri(_API_URL);
            string apiUrl = fullUrl.Host;
            Ping requestServer = new Ping();
            PingReply serverResponse = requestServer.Send(apiUrl, 10000);

            if (serverResponse.Status == IPStatus.Success)
            {
                Console.WriteLine("IP Address: {0}", serverResponse.Address.ToString());
                Console.WriteLine("RoundTrip time: {0}", serverResponse.RoundtripTime);
                Console.WriteLine("Time to live: {0}", serverResponse.Options.Ttl);
                Console.WriteLine("Don't fragment: {0}", serverResponse.Options.DontFragment);
                Console.WriteLine("Buffer size: {0}", serverResponse.Buffer.Length);
                return (TStatus)new CosmicLatteStatus(true);
            }
            else
                Console.WriteLine(serverResponse.Status);
                return (TStatus)new CosmicLatteStatus(false);
            */
        }
    }
}
