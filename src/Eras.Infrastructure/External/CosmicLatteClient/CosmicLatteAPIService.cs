using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Domain.Services;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using Answers = Eras.Application.Dtos.Answers;
using ComponentVariable = Eras.Domain.Entities.ComponentVariable;

namespace Eras.Infrastructure.External.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService
    {
        private const string PathEvalaution = "evaluations";
        private const string HeaderApiKey = "x-apikey";
        private string _apiKey;
        private string _apiUrl;
        private readonly HttpClient _httpClient;

        private CosmicLatteMapper cosmicLatteMapper = new CosmicLatteMapper();

        private IStudentService _studentService = new StudentService();
        private IPollService _pollService = new PollService();
        private IComponentVariableService _componentVariableService = new ComponentVariableService();
        private IAnswerService _answerService = new AnswerService();

        public CosmicLatteAPIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _apiKey = configuration.GetSection("CosmicLatte:ApiKey").Value;
            _apiUrl = configuration.GetSection("CosmicLatte:BaseUrl").Value;
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


        public async Task<string> ImportAllPolls(string name, string startDate, string endDate)
        {
            string path = _apiUrl + PathEvalaution;
            if (name != null || startDate != null || endDate != null)
            {
                // ?$filter=contains(name, 'Encuesta') and startedAt ge 2024-12-24T14:22:47.913Z and startedAt le 2025-12-24T14:23:47.913Z
                // verificar casos, existe la posibilidad que no me manden el nombre? tipo pidiendo TODA LA INFO?
                path += "?$filter=";
                if (name != null) path += $"contains(name,'{name}')"; // Chequear: SI paso un solo caracter "E", no me toma la api, pero si mando "En" si.. Por?
                if (startDate != null) path += $" and startedAt ge {ConvertStringToIsoExtendedDate(startDate)}";
                if (endDate != null) path += $" and startedAt le {ConvertStringToIsoExtendedDate(endDate)}";
            }
            // TODO hay un error en el mapeo
            // sucede cuando se envia la peticion sin ningun parametro, Cosmic responde correctamente, pero al mappear se produce un error.
            // Este error no se produce si solo pedimos la ultima encuesta creada..
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody);

                List<DataItem> cosmicLattePollList = apiResponse.data; // lista de todas las respuestas a la poll

                for(int i = 0; i < cosmicLattePollList.Count; i++)
                {
                    if (i == 0)
                    {
                        // In the first one, Create poll, validate if it is already created before mapping.. but it shouldn`t
                        Poll poll = _pollService.CreatePoll(cosmicLatteMapper.CLtoPoll(cosmicLattePollList[i]));
                        CreateVariablesByPollResponseId(cosmicLattePollList[i]._id, poll); // get, create and save variables requesting endpoint CL
                    }
                    // Obtengo lista de de respuestas a dicho poll y la paso a metodo que llame para obtener info
                    // , deberia pasar un ID + lista de answer y question textuales
                    ImportPollById(cosmicLattePollList[i]._id, cosmicLattePollList[i].score);
                }


                return "";
            }
            catch (Exception e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        public async Task<string> CreateVariablesByPollResponseId(string id, Poll poll)
        {
            // ID example kY96D2446fHJupvod
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{id}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody);

                // For each answer, create a variable, save and set relation. It includes 
                foreach (var item in apiResponse.Data.Answers)
                {
                    ComponentVariable variable = _componentVariableService.CreateVariable(cosmicLatteMapper.CLtoVariable(item.Value, poll.Id));
                }
                return "";
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }

        }
        public async Task<string> ImportPollById(string id, Score scoreItem)
        {
            // ID example kY96D2446fHJupvod
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{id}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody);

                // Create student, save and return  (or find )
                Student student = _studentService.CreateStudent(cosmicLatteMapper.CLtoStudent(apiResponse));

                // Create answer for each answer from CL
                foreach(var item in apiResponse.Data.Answers)
                {
                    Answer answer = _answerService.CreateAnswer(cosmicLatteMapper.CLtoAnswer(item.Value), student);

                }
                return "";
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        private string ConvertStringToIsoExtendedDate(string date)
        {
            string[] parts = date.Split('-');
            int year = int.Parse(parts[0]);
            int month = parts.Length > 1 ? int.Parse(parts[1]) : 1;
            int day = parts.Length > 2 ? int.Parse(parts[2]) : 1;
            DateTime dateFromDate = new DateTime(year, month, day);
            return dateFromDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

    }
}