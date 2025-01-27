using Eras.Application.Dtos;
using Eras.Application.Mappers;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Domain.Services;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.Extensions.Configuration;
using System.Data;
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

        private readonly IStudentService _studentService;
        private IComponentVariableService _componentVariableService;
        private IPollService _pollService;
        private IAnswerService _answerService;

        public CosmicLatteAPIService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IStudentService studentService,
            IPollService pollService,
            IComponentVariableService componentVariableService,
            IAnswerService answerService)
        {
            _apiKey = configuration.GetSection("CosmicLatte:ApiKey").Value ?? throw new Exception("Cosmic latte api key not found");
            _apiUrl = configuration.GetSection("CosmicLatte:BaseUrl").Value ?? throw new Exception("Cosmic latte Url not found");
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _studentService = studentService;
            _pollService = pollService;
            _componentVariableService = componentVariableService;
            _answerService = answerService;
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


        public async Task<string?> ImportAllPolls(string name, string startDate, string endDate)
        {
            string path = _apiUrl + PathEvalaution;
            if (name != null || startDate != null || endDate != null)
            {
                path += "?$filter=";
                if (name != null) path += $"contains(name,'{name}')";
                if (startDate != null) path += $" and startedAt ge {ConvertStringToIsoExtendedDate(startDate)}";
                if (endDate != null) path += $" and startedAt le {ConvertStringToIsoExtendedDate(endDate)}";
            }
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody)?? throw new Exception("Unable to deserialize response from cosmic latte");

                List<DataItem> cosmicLattePollList = apiResponse.data;

                for(int i = 0; i < cosmicLattePollList.Count; i++)
                {
                    if (i == 0)
                    {
                        // In the first one, Create poll, validate if it is already created before mapping.. but it shouldn`t
                        Poll poll = _pollService.CreatePoll(CosmicLatteMapper.ToPoll(cosmicLattePollList[i])).Result;
                        await CreateVariablesByPollResponseId(cosmicLattePollList[i]._id, poll); // get, create and save variables requesting endpoint CL
                    }
                    await ImportPollById(cosmicLattePollList[i]._id, cosmicLattePollList[i].score);
                }
                return "";
            }
            catch (Exception e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        public async Task CreateVariablesByPollResponseId(string id, Poll poll)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{id}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                // For each answer, create a variable, save and set relation. It includes 
                foreach (var item in apiResponse.Data.Answers)
                {
                    ComponentVariable variable = _componentVariableService.CreateVariable(CosmicLatteMapper.ToVariable(item.Value, poll.Id)).Result;
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }

        }
        public async Task ImportPollById(string id, Score scoreItem)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{id}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                // Create student, save and return  (or find )
                Student student = await _studentService.CreateStudent(CosmicLatteMapper.ToStudent(apiResponse));

                // Create answer for each answer from CL
                // We should create something like this: await _answerService.CreateAnswersForStudent(apiResponse.Data.Answers, student, scoreItem);
                // but it requieres mappers and other logic to mantain layers
                foreach (var item in apiResponse.Data.Answers)
                {
                    Answers answersCl = item.Value;
                    answersCl.Score = GetScoreByPositionAndAnswer(answersCl.Position, scoreItem);
                    Answer answer = await _answerService.CreateAnswer(CosmicLatteMapper.ToAnswer(answersCl), student);
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        private double GetScoreByPositionAndAnswer(int position, Score scoreItem)
        {
            ByPosition byPositionItem = scoreItem.byPosition.Find(ans => ans.position == position);
            if (byPositionItem != null) return byPositionItem.score;
            return 0;
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