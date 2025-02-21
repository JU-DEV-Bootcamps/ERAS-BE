using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;


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
        public async Task<List<PollDTO>> ImportAllPolls(string name, string startDate, string endDate)
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
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseModelForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                Dictionary<string, List<int>> variablesPositionByComponents = GetListOfVariablePositionByComponents(apiResponse.data[0]);
                // 1. Create components and variables
                List<ComponentDTO> componentsAndVariables = GetComponentsAndVariables(apiResponse.data[0]._id, variablesPositionByComponents).Result;

                List<PollDTO> pollsDtos = new List<PollDTO>();
                foreach (var responseToPollInstace in apiResponse.data)
                {
                    if (responseToPollInstace.status == "validated")
                    {
                        ICollection<ComponentDTO> populatedComponents = await PopulateListOfComponentsByIdPollInstance(componentsAndVariables, responseToPollInstace._id, responseToPollInstace.score);

                        if(populatedComponents != null)
                        {
                            // 2. Create polls
                            string version = responseToPollInstace.parent + "-" + responseToPollInstace.changeHistory.Last().when; // TO REVIEW

                            PollDTO pollDto = new PollDTO
                            {
                                Name = responseToPollInstace.name,
                                Version = version,
                                Components = populatedComponents,
                                FinishedAt = responseToPollInstace.finishedAt
                            };
                            pollsDtos.Add(pollDto);
                        }
                    }
                }
                // At this point we have created a huge json with a lot of duplicate information, it makes no sense.
                // We should redesign the next layer so that this transfer of duplicate information is not required.
                CreateComandResponse<Poll> createdPollResponse = await _pollOrchestratorService.ImportPollInstances(pollsDtos);
                await _pollOrchestratorService.ImportPollInstances(pollsDtos);
                return pollsDtos;
            }
            catch (Exception e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
        public async Task<List<ComponentDTO>> PopulateListOfComponentsByIdPollInstance(List<ComponentDTO> components, string pollId, Score scoreItem)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{pollId}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();


                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody) ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

                string studentName = apiResponse.Data.Answers.ElementAt(0).Value.AnswersList[0];
                string studentEmail = apiResponse.Data.Answers.ElementAt(1).Value.AnswersList[0];
                string studentCohort = apiResponse.Data.Answers.ElementAt(2).Value.AnswersList[0];
                StudentDTO studentDto = CreateStudent(studentName, studentEmail, studentCohort); 

                // clone list
                List<ComponentDTO> clonedListComponents = components.Select( c => new ComponentDTO
                {
                    Name = c.Name,
                    Variables = c.Variables.Select(variable => new VariableDTO
                    {
                        Name = variable.Name,
                        Position = variable.Position,
                        Type = variable.Type,
                        Answer = new AnswerDTO(),
                        Audit = variable.Audit,
                    }).ToList(),
                    Audit = c.Audit,
                }).ToList();

                foreach (KeyValuePair<int, Answers> answerCL in apiResponse.Data.Answers)  
                {
                    foreach (ComponentDTO component in clonedListComponents)
                    {
                        foreach(VariableDTO variable in component.Variables)
                        {
                            if (variable.Position == answerCL.Value.Position)
                            {
                                variable.Answer = CreateAnswer(answerCL, studentDto, scoreItem);                                  
                            }
                        }

                    }

                }
                return clonedListComponents;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                return null;
            }
        }
        public Dictionary<string, List<int>> GetListOfVariablePositionByComponents(DataItem clDataItem)
        {
            Dictionary<string, JsonElement> traits = clDataItem.score.byTrait.traits;
            return ByTrait.getVariablesPositionByComponents(traits);
        }
        public async Task<List<ComponentDTO>> GetComponentsAndVariables(string pollId, Dictionary<string, List<int>> variablesPositionByComponents)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{pollId}\"}}}}", Encoding.UTF8, "application/json");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();


                CLResponseModelForPollDTO apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody) ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

                List<ComponentDTO> results = [];
                foreach (KeyValuePair<string, List<int>> item in variablesPositionByComponents)
                {
                    // 3. Create variables
                    ICollection<VariableDTO> createdVariables = new List<VariableDTO>();
                    Dictionary<int, Answers> AnswersList = apiResponse.Data.Answers;
                    foreach (var itemVariable in AnswersList)
                    {
                        if (item.Value.Contains(itemVariable.Value.Position))
                        {
                            VariableDTO newVariable = new VariableDTO();
                            newVariable.Name = itemVariable.Value.Question.Body["es"];
                            newVariable.Position = itemVariable.Value.Position;
                            newVariable.Type = itemVariable.Value.Type;
                            newVariable.Answer = null; // CreateAnswer(itemVariable, studentDTO, pollData.score);
                            createdVariables.Add(newVariable);
                        }
                    }
                    // 4. Create components
                    ComponentDTO component = new ComponentDTO { Name = item.Key, Variables = createdVariables };
                    results.Add(component);
                }
                return results;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                return null;
            }

        }
        public StudentDTO CreateStudent(string name, string email, string cohort)
        {
            StudentDTO studentDTO = new StudentDTO { Name = name, Email = email, Uuid = null };
            CohortDTO cohortDTO = new CohortDTO() { Name = cohort};
            studentDTO.Cohort = cohortDTO;
            return studentDTO;
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

        public async Task<List<PollDataItem>> GetPollsNameList()
        {
            try
            {
                string path = _apiUrl + PathEvalaution;
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return null;

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                List<PollDataItem> pollsData = apiResponse.data
                                        .GroupBy(poll => poll.parent)
                                        .Select(poll => new PollDataItem (poll.First().parent, poll.First().name, poll.First().status))
                                        .Distinct()
                                        .ToList();
                return pollsData;
            } catch (Exception e)
            {
                throw new Exception($"Cosmic latte server error: {e.Message}");
            }
        }
    }
}
