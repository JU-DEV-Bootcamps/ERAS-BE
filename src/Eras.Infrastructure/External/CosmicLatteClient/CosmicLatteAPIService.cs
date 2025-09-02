using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Models.Response.Common;
using Eras.Application.Utils;
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
        private readonly HttpClient _httpClient;
        private readonly ILogger<CosmicLatteAPIService> _logger;
        private readonly PollOrchestratorService _pollOrchestratorService;
        private readonly IApiKeyEncryptor _encryptor;

        public CosmicLatteAPIService(
            IConfiguration Configuration,
            IHttpClientFactory HttpClientFactory,
            ILogger<CosmicLatteAPIService> Logger,
            PollOrchestratorService PollOrchestratorService,
            IApiKeyEncryptor Encryptor)
        {
            _httpClient = HttpClientFactory.CreateClient();
            _logger = Logger;
            _pollOrchestratorService = PollOrchestratorService;
            _encryptor = Encryptor;
        }

        public async Task<CosmicLatteStatus> CosmicApiIsHealthy(string ApiKey, string ApiUrl)
        {
            var decryptedApiKey = _encryptor.Decrypt(ApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiUrl}{PathEvalautionSet}?$filter=contains(name,' ')");
            request.Headers.Add(HeaderApiKey, decryptedApiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return new CosmicLatteStatus(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                return new CosmicLatteStatus(false);
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

        public async Task<List<PollDTO>> GetAllPollsPreview(
                string EvaluationSetName,
                string StartDate,
                string EndDate,
                string ApiKey,
                string ApiUrl)
        {
            var decryptedApiKey = _encryptor.Decrypt(ApiKey);
            string evaluationSetId = await GetEvaluationSetIdAsync(EvaluationSetName, decryptedApiKey, ApiUrl);
            string path = $"{ApiUrl}{PathEvalaution}";

            if (!string.IsNullOrEmpty(evaluationSetId))
            {
                path += "?$filter=";
                if (!string.IsNullOrEmpty(evaluationSetId))
                    path += $"contains(parent,'evaluationSets:{evaluationSetId}')";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add(HeaderApiKey, decryptedApiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Cosmic latte server error, Message: {response.ReasonPhrase}");

            string responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody)
                              ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

            var validatedEvaluations = apiResponse.data
                .Where(e => e.status == "validated")
                .ToList();

            if (validatedEvaluations.Count == 0 || validatedEvaluations[0].Id == null)
                return new List<PollDTO>();

            var variablesPositionByComponents = GetListOfVariablePositionByComponents(validatedEvaluations[0]);
            var componentsAndVariables = await GetComponentsAndVariablesAsync(
                validatedEvaluations[0].Id!,
                variablesPositionByComponents,
                decryptedApiKey,
                ApiUrl);

            var pollsDtos = new List<PollDTO>();

            if (componentsAndVariables.Count > 0)
            {
                foreach (var responseToPollInstance in apiResponse.data)
                {
                    var populatedComponents = await PopulateListOfComponentsByIdPollInstanceAsync(
                        componentsAndVariables,
                        responseToPollInstance.Id,
                        responseToPollInstance.score,
                        decryptedApiKey,
                        ApiUrl,
                        StartDate,
                        EndDate);

                    if (populatedComponents.Count > 0)
                    {
                        var pollDto = new PollDTO
                        {
                            IdCosmicLatte = responseToPollInstance.Id ?? string.Empty,
                            Uuid = Guid.NewGuid().ToString(),
                            Name = SqlInjectionValidator.Sanitize(responseToPollInstance.name),
                            FinishedAt = responseToPollInstance.finishedAt,
                            LastVersion = 1,
                            LastVersionDate = DateTime.UtcNow,
                            Components = SanitizeComponents(populatedComponents)
                        };
                        pollsDtos.Add(pollDto);
                    }
                }
            }

            return pollsDtos;
        }


        public async Task<List<ComponentDTO>> PopulateListOfComponentsByIdPollInstanceAsync(
                List<ComponentDTO> Components,
                string? PollId,
                Score? ScoreItem,
                string ApiKey,
                string ApiUrl,
                string StartDate,
                string EndDate)
        {
            if (PollId == null || ScoreItem == null)
            {
                _logger.LogError("Cosmic latte PopulateList error: PollId or ScoreItem is null");
                return new List<ComponentDTO>();
            }

            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{PollId}\"}}}}", Encoding.UTF8, "application/json");

            try
            {
                string path = $"{ApiUrl}{PathEvalaution}/exec/evaluationDetails";
                var request = new HttpRequestMessage(HttpMethod.Post, path);
                request.Content = content;
                request.Headers.Add(HeaderApiKey, ApiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody)
                                  ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

                string studentName = apiResponse.Data.Answers.ElementAt(0).Value.AnswersList[0];
                string studentEmail = apiResponse.Data.Answers.ElementAt(1).Value.AnswersList[0];
                string studentCohort = apiResponse.Data.Answers.ElementAt(2).Value.AnswersList[0];
                StudentDTO studentDto = CreateStudent(studentName, studentEmail, studentCohort);
                List<ComponentDTO> clonedListComponents = new List<ComponentDTO>();

                if(string.IsNullOrEmpty(StartDate) &&
                    string.IsNullOrEmpty(EndDate))
                {
                    clonedListComponents = CloneComponentsList(Components);
                }else if (!string.IsNullOrEmpty(StartDate) && 
                    !string.IsNullOrEmpty(EndDate) && 
                    CohortsHelper.CohortInDateRange(studentCohort ,DateTime.Parse(StartDate), DateTime.Parse(EndDate))
                    )
                {
                    clonedListComponents = CloneComponentsList(Components);
                }
                else if(!string.IsNullOrEmpty(StartDate) &&
                    string.IsNullOrEmpty(EndDate) &&
                    CohortsHelper.GetCohort(DateTime.Parse(StartDate)) == studentCohort
                    )
                {
                    clonedListComponents = CloneComponentsList(Components);
                }

                foreach (var answerCL in apiResponse.Data.Answers)
                {
                    foreach (var component in clonedListComponents)
                    {
                        foreach (var variable in component.Variables)
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

        public static List<ComponentDTO> CloneComponentsList(List<ComponentDTO> ComponentsList)
        {
            var clonedListComponents = ComponentsList.Select(c => new ComponentDTO
            {
                Name = c.Name,
                Variables = c.Variables.Select(v => new VariableDTO
                {
                    Name = v.Name,
                    Position = v.Position,
                    Type = v.Type,
                    Answer = new AnswerDTO(),
                    Audit = v.Audit,
                    Version = v.Version
                }).ToList(),
                Audit = c.Audit
            }).ToList();
            return clonedListComponents;
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
        public async Task<List<ComponentDTO>> GetComponentsAndVariablesAsync(
            string PollId,
            Dictionary<string, List<int>> VariablesPositionByComponents,
            string ApiKey,
            string ApiUrl)
        {
            var content = new StringContent($"{{\"@data\":{{\"_id\":\"{PollId}\"}}}}", Encoding.UTF8, "application/json");

            try
            {
                string path = $"{ApiUrl}{PathEvalaution}/exec/evaluationDetails";
                var request = new HttpRequestMessage(HttpMethod.Post, path);
                request.Content = content;
                request.Headers.Add(HeaderApiKey, ApiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("Unsuccessful response from cosmic latte");

                string responseBody = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody)
                                  ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

                var results = new List<ComponentDTO>();
                var answersList = apiResponse.Data.Answers;
                var processedPositions = new HashSet<int>();

                foreach (var item in VariablesPositionByComponents)
                {
                    var createdVariables = new List<VariableDTO>();

                    foreach (var itemVariable in answersList)
                    {
                        if (item.Value.Contains(itemVariable.Value.Position))
                        {
                            var newVariable = new VariableDTO
                            {
                                Name = itemVariable.Value.Question.Body["es"],
                                Position = itemVariable.Value.Position,
                                Type = itemVariable.Value.Type,
                                Answer = null,
                                Version = new VersionInfo()
                            };
                            createdVariables.Add(newVariable);
                            processedPositions.Add(itemVariable.Value.Position);
                        }
                    }

                    var component = new ComponentDTO
                    {
                        Name = item.Key,
                        Variables = createdVariables
                    };

                    results.Add(component);
                }

                ProcessOpenEndedQuestions(results, answersList, processedPositions, VariablesPositionByComponents);
                SortVariablesByPosition(results);

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
                if (AnswersKVPair.Value.AnswersList.Length > 1 &&
                    Array.IndexOf(AnswersKVPair.Value.AnswersList, answers) != (AnswersKVPair.Value.AnswersList.Length - 1))
                    answerSB.Append("; ");
            }

            decimal score;
            if (IsOpenEndedQuestion(AnswersKVPair.Value.Type))
            {
                score = 0m;
            }
            else
            {
                score = GetScoreByPositionAndAnswer(AnswersKVPair.Value.Position, ScoreItem);
            }

            score = AnswersKVPair.Value.AnswersList.Length > 0 ? score / AnswersKVPair.Value.AnswersList.Length : score;
            return new AnswerDTO
            {
                Answer = answerSB.ToString(),
                Score = Math.Round(score, 2),
                Student = Student,
                Version = new VersionInfo()
            };
        }
        private static decimal GetScoreByPositionAndAnswer(int Position, Score ScoreItem)
        {
            ByPosition? byPositionItem = ScoreItem.byPosition.Find(Ans => Ans.position == Position);
            if (byPositionItem != null) return byPositionItem.score;
            return 0;
        }

        private static bool IsOpenEndedQuestion(string QuestionType)
        {
            return QuestionType == "openTextSingleline" || QuestionType == "openTextMultiline";
        }

        private static string DetermineComponentForOpenEndedQuestion(int Position, Dictionary<string, List<int>> ComponentsMap)
        {
            foreach (var component in ComponentsMap)
            {
                var positions = component.Value;
                if (positions.Count > 0)
                {
                    int minPos = positions.Min();
                    int maxPos = positions.Max();
                    
                    if (Position >= minPos && Position <= maxPos)
                    {
                        return component.Key;
                    }
                }
            }
            
            return "personalData";
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

        public async Task<List<PollDataItem>> GetPollsNameList(string BaseUrl, string ApiKey)
        {
            try
            {
                var decryptedApiKey = _encryptor.Decrypt(ApiKey);
                string path = BaseUrl + PathEvalautionSet + "?$top=100";
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, decryptedApiKey);

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

        private async Task<string> GetEvaluationSetIdAsync(string EvaluationName, string ApiKey, string ApiUrl)
        {
            string pathEvaluationSet = $"{ApiUrl}{PathEvalautionSet}?$top=100";
            var requestEvaluationSet = new HttpRequestMessage(HttpMethod.Get, pathEvaluationSet);
            requestEvaluationSet.Headers.Add(HeaderApiKey, ApiKey);

            try
            {
                var responseEvaluationSet = await _httpClient.SendAsync(requestEvaluationSet);

                if (!responseEvaluationSet.IsSuccessStatusCode)
                    throw new Exception($"Cosmic latte server error, Message: {responseEvaluationSet.ReasonPhrase}");

                var contentResponseEvaluationSet = await responseEvaluationSet.Content.ReadAsStringAsync();
                var evaluationSets = JsonSerializer.Deserialize<CLEvaluationSetDTOList>(contentResponseEvaluationSet);

                if (evaluationSets == null)
                    throw new Exception("Evaluation sets not found");

                var evaluationSet = evaluationSets.data.Find(e => e.name == EvaluationName);
                if (evaluationSet == null)
                    throw new Exception($"Evaluation not found: {EvaluationName}");

                return evaluationSet._id;
            }
            catch (Exception e)
            {
                throw new Exception($"There was an error with the request: {e.Message}");
            }
        }

        private static List<ComponentDTO> SanitizeComponents(List<ComponentDTO> components)
        {
            foreach (var component in components)
            {
                component.Name = SqlInjectionValidator.Sanitize(component.Name);
                
                foreach (var variable in component.Variables)
                {
                    variable.Name = SqlInjectionValidator.Sanitize(variable.Name);
                    
                    if (!string.IsNullOrEmpty(variable.Type))
                    {
                        variable.Type = SqlInjectionValidator.Sanitize(variable.Type);
                    }
                    
                    if (variable.Answer != null && !string.IsNullOrEmpty(variable.Answer.Answer))
                    {
                        variable.Answer.Answer = SqlInjectionValidator.Sanitize(variable.Answer.Answer);
                    }
                }
            }
            return components;
        }

        private void ProcessOpenEndedQuestions(
            List<ComponentDTO> Results, 
            Dictionary<int, Answers> AnswersList, 
            HashSet<int> ProcessedPositions, 
            Dictionary<string, List<int>> VariablesPositionByComponents)
        {
            var openEndedByComponent = CollectOpenEndedQuestionsByComponent(AnswersList, ProcessedPositions, VariablesPositionByComponents);
            IntegrateOpenEndedQuestionsIntoComponents(Results, openEndedByComponent);
        }

        private Dictionary<string, List<VariableDTO>> CollectOpenEndedQuestionsByComponent(
            Dictionary<int, Answers> AnswersList, 
            HashSet<int> ProcessedPositions, 
            Dictionary<string, List<int>> VariablesPositionByComponents)
        {
            var openEndedByComponent = new Dictionary<string, List<VariableDTO>>();

            foreach (var itemVariable in AnswersList)
            {
                if (ShouldProcessOpenEndedQuestion(itemVariable, ProcessedPositions))
                {
                    var newVariable = CreateVariableFromAnswer(itemVariable);
                    var targetComponent = DetermineComponentForOpenEndedQuestion(itemVariable.Value.Position, VariablesPositionByComponents);
                    
                    AddVariableToComponentGroup(openEndedByComponent, targetComponent, newVariable);
                }
            }

            return openEndedByComponent;
        }

        private void IntegrateOpenEndedQuestionsIntoComponents(List<ComponentDTO> Results, Dictionary<string, List<VariableDTO>> OpenEndedByComponent)
        {
            foreach (var openEndedGroup in OpenEndedByComponent)
            {
                string componentName = openEndedGroup.Key;
                var openEndedVariables = openEndedGroup.Value;
                
                if (componentName == "personalData")
                {
                    continue;
                }
                
                var existingComponent = Results.FirstOrDefault(c => c.Name == componentName);
                if (existingComponent != null)
                {
                    AddVariablesToExistingComponent(existingComponent, openEndedVariables);
                }
                else
                {
                    CreateAndAddNewComponent(Results, componentName, openEndedVariables);
                }
            }
        }

        private static bool ShouldProcessOpenEndedQuestion(KeyValuePair<int, Answers> ItemVariable, HashSet<int> ProcessedPositions)
        {
            return !ProcessedPositions.Contains(ItemVariable.Value.Position) && 
                   IsOpenEndedQuestion(ItemVariable.Value.Type);
        }

        private static VariableDTO CreateVariableFromAnswer(KeyValuePair<int, Answers> ItemVariable)
        {
            return new VariableDTO
            {
                Name = ItemVariable.Value.Question.Body["es"],
                Position = ItemVariable.Value.Position,
                Type = ItemVariable.Value.Type,
                Answer = null,
                Version = new VersionInfo()
            };
        }

        private static void AddVariableToComponentGroup(Dictionary<string, List<VariableDTO>> OpenEndedByComponent, string TargetComponent, VariableDTO Variable)
        {
            if (!OpenEndedByComponent.ContainsKey(TargetComponent))
            {
                OpenEndedByComponent[TargetComponent] = new List<VariableDTO>();
            }
            
            OpenEndedByComponent[TargetComponent].Add(Variable);
        }


        private static void AddVariablesToExistingComponent(ComponentDTO ExistingComponent, List<VariableDTO> OpenEndedVariables)
        {
            foreach (var variable in OpenEndedVariables)
            {
                ExistingComponent.Variables.Add(variable);
            }
        }

        private static void CreateAndAddNewComponent(List<ComponentDTO> Results, string ComponentName, List<VariableDTO> OpenEndedVariables)
        {
            var newComponent = new ComponentDTO
            {
                Name = ComponentName,
                Variables = OpenEndedVariables
            };
            Results.Add(newComponent);
        }

        private static void SortVariablesByPosition(List<ComponentDTO> Results)
        {
            foreach (var component in Results)
            {
                component.Variables = component.Variables.OrderBy(v => v.Position).ToList();
            }
        }
    }
}
