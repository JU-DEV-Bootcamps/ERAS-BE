using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.Features.Polls.Commands.CreatePoll;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Text.Json;


namespace Eras.Infrastructure.External.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService
    {
        private const string PathEvalaution = "evaluations";
        private const string HeaderApiKey = "x-apikey";
        private string _apiKey;
        private string _apiUrl;
        private readonly HttpClient _httpClient;

        private readonly ILogger<CosmicLatteAPIService> _logger;
        private readonly PollOrchestratorService _pollOrchestratorService;

        public CosmicLatteAPIService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<CosmicLatteAPIService> logger,
            PollOrchestratorService pollOrchestratorService )
        {
            _apiKey = configuration.GetSection("CosmicLatte:ApiKey").Value ?? throw new Exception("Cosmic latte api key not found"); // this should be move to .env
            _apiUrl = configuration.GetSection("CosmicLatte:BaseUrl").Value ?? throw new Exception("Cosmic latte Url not found"); // this should be move to .env
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _logger = logger;
            _pollOrchestratorService = pollOrchestratorService;
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
                throw new Exception($"There was an error with the request: " + e.Message);
            }
        }
        public async Task<int> ImportAllPolls(string name, string startDate, string endDate)
        {
            string path = _apiUrl + PathEvalaution;
            if (name != "" || startDate != "" || endDate != "")
            {
                path += "?$filter=";
                if (name != "") path += $"contains(name,'{name}')"; // TODO check exist a scenario where no name is sent?
                if (startDate != "") path += $" and startedAt ge {ConvertStringToIsoExtendedDate(startDate)}";
                if (endDate != "") path += $" and startedAt le {ConvertStringToIsoExtendedDate(endDate)}";
            }
            try
            {
                int newRegisters = 0;
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return 0;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                if (apiResponse.data.Count > 0)
                {
                    Dictionary<string, List<int>> variablesPositionByComponents = GetListOfVariablePositionByComponents(apiResponse.data[0]);

                    // 1. Create components
                    ICollection<ComponentDTO> components = CreateComponents(apiResponse.data[0], variablesPositionByComponents);

                    // 2. Create polls
                    string version = apiResponse.data[0].parent + "-" + apiResponse.data[0].changeHistory.Last().when; // TO REVIEW

                    PollDTO pollDto = new PollDTO
                    {
                        Name = apiResponse.data[0].name,
                        Version = version,
                        Components = components,
                    };
                    BaseResponse createdPollResponse = await _pollOrchestratorService.ImportPoll(pollDto);
                    if (createdPollResponse.Success) newRegisters++;
                }
                return newRegisters;
            }
            catch (Exception e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        public Dictionary<string, List<int>> GetListOfVariablePositionByComponents(DataItem clDataItem)
        {
            Dictionary<string, JsonElement> traits = clDataItem.score.byTrait.traits;
            return ByTrait.getVariablesPositionByComponents(traits);
        }
        public List<ComponentDTO> CreateComponents(DataItem pollData, Dictionary<string, List<int>> variablesPositionByComponents) // { "academico" : [1,2,3], "socioeconomico" : [4,5,6]}
        {
            List<ComponentDTO> results = [];
            foreach (KeyValuePair<string, List<int>> item in variablesPositionByComponents)
            {
                // 3. Create variables
                ICollection<VariableDTO> variables = CreateVariablesByComponentAndPoll(pollData, item.Value).Result;
                // 4. Create components
                ComponentDTO component = new ComponentDTO { Name = item.Key, Variables = variables };
                results.Add(component);
            }
            return results;
        }
        public async Task<List<VariableDTO>> CreateVariablesByComponentAndPoll(DataItem pollData, List<int> positionsByComponent)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{pollData._id}\"}}}}", Encoding.UTF8, "application/json");

            List<VariableDTO> createdVariables = new List<VariableDTO>();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");


                string studentName = apiResponse.Data.Answers.ElementAt(0).Value.AnswersList[0];
                string studentEmail = apiResponse.Data.Answers.ElementAt(1).Value.AnswersList[0];

                StudentDTO studentDTO = CreateStudent(studentName, studentEmail);

                foreach (var itemVariable in apiResponse.Data.Answers)
                {
                    if (positionsByComponent.Contains(itemVariable.Value.Position))
                    {
                        VariableDTO newVariable = new VariableDTO();
                        newVariable.Name = itemVariable.Value.Question.Body["es"];
                        newVariable.Position = itemVariable.Value.Position;
                        newVariable.Type = itemVariable.Value.Type;
                        newVariable.Answer = CreateAnswer(itemVariable, studentDTO, pollData.score);
                        createdVariables.Add(newVariable);
                    }
                }
                return createdVariables;
            }
            catch (HttpRequestException e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        public StudentDTO CreateStudent(string name, string email)
        {
            return new StudentDTO { Name = name, Email = email, Uuid = null };
        }
        public AnswerDTO CreateAnswer(KeyValuePair<int, Answers> answersKVPair, StudentDTO student, Score scoreItem)
        {
            StringBuilder answerSB = new StringBuilder();
            foreach (string answers in answersKVPair.Value.AnswersList)
            {
                answerSB.Append(answers);
            }
            double score = GetScoreByPositionAndAnswer(answersKVPair.Value.Position, scoreItem);
            return new AnswerDTO { Answer = answerSB.ToString(), Score = score, Student = student };
        }
        private static double GetScoreByPositionAndAnswer(int position, Score scoreItem)
        {
            ByPosition? byPositionItem = scoreItem.byPosition.Find(ans => ans.position == position);
            if (byPositionItem != null) return byPositionItem.score;
            return 0;
        }
        private static string ConvertStringToIsoExtendedDate(string date)
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
