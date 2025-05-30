using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.External.CosmicLatteClient
{
    [ExcludeFromCodeCoverage]
    public class CosmicLatteAPIService : ICosmicLatteAPIService
    {
        private const string PathEvalautionSet = "evaluationSets";
        private const string PathEvalaution = "evaluations";
        private const string HeaderApiKey = "x-apikey";
        private string _apiKey;
        private string _apiUrl;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CosmicLatteAPIService> _logger;
        private readonly PollOrchestratorService _pollOrchestratorService;

        public CosmicLatteAPIService(
            IConfiguration Configuration,
            IHttpClientFactory HttpClientFactory,
            ILogger<CosmicLatteAPIService> Logger,
            PollOrchestratorService PollOrchestratorService)
        {
            _apiKey = Configuration.GetSection("CosmicLatte:ApiKey").Value ?? throw new Exception("Cosmic latte api key not found");
            _apiUrl = Configuration.GetSection("CosmicLatte:BaseUrl").Value ?? throw new Exception("Cosmic latte Url not found");
            _httpClient = HttpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_apiUrl);
            _logger = Logger;
            _pollOrchestratorService = PollOrchestratorService;
        }
        public async Task<CosmicLatteStatus> CosmicApiIsHealthy()
        {
            string path = _apiUrl + PathEvalautionSet;
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


        public async Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> PollsDtos)
        {
            try
            {
                CreateCommandResponse<CreatedPollDTO> createdPoll = await _pollOrchestratorService.ImportPollInstancesAsync(PollsDtos);

                if (createdPoll.Entity == null)
                {
                    _logger.LogError("Error saving data: createdPoll is null");
                    throw new Exception($"Error saving data: createdPoll is null");

                }
                return createdPoll.Entity;
            }
            catch (Exception e)
            {
                throw new Exception($"Error saving data: {e.Message}");
            }
        }

        public async Task<List<PollDTO>> GetAllPollsPreview(string EvaluationSetName, string StartDate, string EndDate)
        {
            string evaluationSetId = "";
            evaluationSetId = await GetEvaluationSetIdAsync(EvaluationSetName);
            string path = _apiUrl + PathEvalaution;

            if (evaluationSetId != "" || StartDate != "" || EndDate != "")
            {
                path += "?$filter=";
                if (evaluationSetId != "") path += $"contains(parent,'evaluationSets:{evaluationSetId}')";
                if (StartDate != "") path += $" and startedAt ge {ConvertStringToIsoExtendedDate(StartDate)}";
                if (EndDate != "") path += $" and startedAt le {ConvertStringToIsoExtendedDate(EndDate)}";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add(HeaderApiKey, _apiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) throw new Exception($"Cosmic latte server error, Message: {response.ReasonPhrase}");

            List<PollDTO> pollsDtos = new List<PollDTO>();

            string responseBody = await response.Content.ReadAsStringAsync();
            CLResponseModelForAllPollsDTO? apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody);
                if (apiResponse == null)
                    throw new InvalidCastException("Unable to deserialize response from cosmic latte");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                throw new InvalidCastException("Unable to deserialize response from cosmic latte");
            }
            if (apiResponse.data.Count == 0)
                return pollsDtos;


            List<DataItem> validatedEvaluations = apiResponse.data.Where(E => E.status == "validated").ToList();

            if (validatedEvaluations.Count == 0 || validatedEvaluations[0].Id == null) return pollsDtos;

            Dictionary<string, List<int>> variablesPositionByComponents = GetListOfVariablePositionByComponents(validatedEvaluations[0]);

            List<ComponentDTO> componentsAndVariables = GetComponentsAndVariablesAsync(validatedEvaluations[0].Id!, variablesPositionByComponents).Result;

            if (componentsAndVariables.Count > 0)
            {
                foreach (var responseToPollInstace in apiResponse.data)
                {
                    ICollection<ComponentDTO> populatedComponents = await PopulateListOfComponentsByIdPollInstanceAsync(componentsAndVariables, responseToPollInstace.Id, responseToPollInstace.score);

                    if (populatedComponents.Count > 0)
                    {
                        // 2. Create polls
                        string version = $"{responseToPollInstace.parent}-{responseToPollInstace.changeHistory.Last().when}";

                        PollDTO pollDto = new PollDTO
                        {
                            Name = responseToPollInstace.name,
                            Components = populatedComponents,
                            FinishedAt = responseToPollInstace.finishedAt
                        };
                        pollsDtos.Add(pollDto);
                    }

                }
            }
            return pollsDtos;
        }
        public async Task<List<ComponentDTO>> PopulateListOfComponentsByIdPollInstanceAsync(List<ComponentDTO> Components, string? PollId, Score? ScoreItem)
        {
            if (PollId == null || ScoreItem == null)
            {
                _logger.LogError($"Cosmic latte PopulateList error: PollId or ScoreItem is null");
                return new List<ComponentDTO>();
            }

            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{PollId}\"}}}}", Encoding.UTF8, "application/json");
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, PathEvalaution + "/exec/evaluationDetails");
                request.Content = content;
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();

                CLResponseModelForPollDTO apiResponse;
                try
                {
                    var deserializeResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody);
                    if (deserializeResponse == null) throw new Exception();
                    else apiResponse = deserializeResponse;
                }
                catch (Exception e)
                {
                    this._logger.LogError("Error Deserializing Components" + e.Message);
                    throw new InvalidCastException("Unable to deserialize response from cosmic latte");
                }

                string studentName = apiResponse.Data.Answers.ElementAt(0).Value.AnswersList[0];
                string studentEmail = apiResponse.Data.Answers.ElementAt(1).Value.AnswersList[0];
                string studentCohort = apiResponse.Data.Answers.ElementAt(2).Value.AnswersList[0];
                StudentDTO studentDto = CreateStudent(studentName, studentEmail, studentCohort);

                // clone list
                List<ComponentDTO> clonedListComponents = Components.Select(C => new ComponentDTO
                {
                    Name = C.Name,
                    Variables = C.Variables.Select(Variable => new VariableDTO
                    {
                        Name = Variable.Name,
                        Position = Variable.Position,
                        Type = Variable.Type,
                        Answer = new AnswerDTO(),
                        Audit = Variable.Audit,
                        Version = Variable.Version
                    }).ToList(),
                    Audit = C.Audit,
                }).ToList();

                foreach (KeyValuePair<int, Answers> answerCL in apiResponse.Data.Answers)
                {
                    foreach (ComponentDTO component in clonedListComponents)
                    {
                        foreach (VariableDTO variable in component.Variables)
                        {
                            if (variable.Position == answerCL.Value.Position)
                            {
                                variable.Answer = CreateAnswer(answerCL, studentDto, ScoreItem);
                            }
                        }
                    }
                }
                return clonedListComponents;
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                return new List<ComponentDTO>();
            }
        }
        public Dictionary<string, List<int>> GetListOfVariablePositionByComponents(DataItem ClDataItem)
        {
            try
            {
                Dictionary<string, JsonElement>? traits = ClDataItem?.score?.byTrait?.Traits;
                if (traits != null)
                {
                    return ByTrait.getVariablesPositionByComponents(traits);
                }
                _logger.LogError($"Cosmic latte server error: Invalid poll");
                return new Dictionary<string, List<int>>();
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                throw new InvalidCastException("Invalid Cosmic Latte poll, not supported for this version.");
            }
        }
        public async Task<List<ComponentDTO>> GetComponentsAndVariablesAsync(string PollId, Dictionary<string, List<int>> VariablesPositionByComponents)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{PollId}\"}}}}", Encoding.UTF8, "application/json");

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
                foreach (KeyValuePair<string, List<int>> item in VariablesPositionByComponents)
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
                            newVariable.Answer = null;
                            newVariable.Version = new VersionInfo();
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
                return new List<ComponentDTO>();
            }

        }
        public StudentDTO CreateStudent(string Name, string Email, string Cohort)
        {
            StudentDTO studentDTO = new StudentDTO { Name = Name, Email = Email, Uuid = string.Empty };
            CohortDTO cohortDTO = new CohortDTO() { Name = Cohort };
            studentDTO.Cohort = cohortDTO;
            return studentDTO;
        }
        public AnswerDTO CreateAnswer(KeyValuePair<int, Answers> AnswersKVPair, StudentDTO Student, Score ScoreItem)
        {
            StringBuilder answerSB = new StringBuilder();
            foreach (string answers in AnswersKVPair.Value.AnswersList)
            {
                answerSB.Append(answers);
            }
            double score = GetScoreByPositionAndAnswer(AnswersKVPair.Value.Position, ScoreItem);
            return new AnswerDTO { Answer = answerSB.ToString(), Score = score, Student = Student, Version = new VersionInfo()};
        }
        private static double GetScoreByPositionAndAnswer(int Position, Score ScoreItem)
        {
            ByPosition? byPositionItem = ScoreItem.byPosition.Find(Ans => Ans.position == Position);
            if (byPositionItem != null) return byPositionItem.score;
            return 0;
        }
        private static string ConvertStringToIsoExtendedDate(string Date)
        {
            string[] parts = Date.Split('-');
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
                string path = _apiUrl + PathEvalautionSet;
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, _apiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new List<PollDataItem>();

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                List<PollDataItem> pollsData = [.. apiResponse.data.Select(Poll => new PollDataItem(Poll.parent, Poll.name, Poll.status))];
                return pollsData;
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"Invalid Cosmic Latte poll, not supported for this version. {e.Message}");
            }
        }

        private async Task<string> GetEvaluationSetIdAsync(string EvaluationName)
        {
            string pathEvaluationSet = _apiUrl + PathEvalautionSet;
            var requestEvaluationSet = new HttpRequestMessage(HttpMethod.Get, pathEvaluationSet);
            requestEvaluationSet.Headers.Add(HeaderApiKey, _apiKey);
            try
            {
                var responseEvaluationSet = await _httpClient.SendAsync(requestEvaluationSet);

                if (!responseEvaluationSet.IsSuccessStatusCode) throw new Exception($"Cosmic latte server error, Message: {responseEvaluationSet.ReasonPhrase}");
                var contentReponseEvaluationSet = await responseEvaluationSet.Content.ReadAsStringAsync();
                CLEvaluationSetDTOList? evaluationSets = JsonSerializer.Deserialize<CLEvaluationSetDTOList>(contentReponseEvaluationSet);
                if (evaluationSets == null)
                    throw new Exception($"Evaluation not found: ");
                var evaluationSet = evaluationSets.data.Find(E => E.name == EvaluationName);
                if (evaluationSet == null)
                    throw new Exception($"Evaluation not found: ");
                return evaluationSet._id;

            }
            catch (Exception e)
            {
                throw new Exception($"There was an error with the request: " + e.Message);
            }
        }
    }
}