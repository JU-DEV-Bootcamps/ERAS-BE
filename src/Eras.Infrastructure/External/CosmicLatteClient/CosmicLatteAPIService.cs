using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.Json;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Models;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Application.Utils;
using Eras.Domain.Common;
using Eras.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.External.CosmicLatteClient
{
    public class CosmicLatteAPIService : ICosmicLatteAPIService
    {
        private const string PathEvaluationSet = "evaluationSets";
        private const string PathEvaluation = "evaluations";
        private const string HeaderApiKey = "x-apikey";
        private static readonly TimeSpan[] RespondentRetryDelays = [TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(400)];
        private readonly HttpClient _httpClient;
        private readonly ILogger<CosmicLatteAPIService> _logger;
        private readonly PollOrchestratorService _pollOrchestratorService;
        private readonly IApiKeyEncryptor _encryptor;
        private readonly IPollInstanceRepository _pollInstanceRepository;

        public CosmicLatteAPIService(
            IConfiguration Configuration,
            IHttpClientFactory HttpClientFactory,
            ILogger<CosmicLatteAPIService> Logger,
            PollOrchestratorService PollOrchestratorService,
            IApiKeyEncryptor Encryptor,
            IPollInstanceRepository PollInstanceRepository)
        {
            _httpClient = HttpClientFactory.CreateClient();
            _logger = Logger;
            _pollOrchestratorService = PollOrchestratorService;
            _encryptor = Encryptor;
            _pollInstanceRepository = PollInstanceRepository;
        }

        public async Task<CosmicLatteStatus> CosmicApiIsHealthy(string ApiKey, string ApiUrl)
        {
            var decryptedApiKey = _encryptor.Decrypt(ApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiUrl}{PathEvaluationSet}?$filter=contains(name,' ')");
            request.Headers.Add(HeaderApiKey, decryptedApiKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                return new CosmicLatteStatus(response.IsSuccessStatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cosmic Latte health check failed for {ApiUrl}", ApiUrl);
                return new CosmicLatteStatus(false);
            }
        }

        public async Task<CreatedPollDTO> SavePreviewPolls(List<PollDTO> PollsDtos, int EvaluationId)
        {
            try
            {
                var invalidPoll = PollsDtos.FirstOrDefault(p => p.Name?.Length > 100);
                if (invalidPoll != null)
                    throw new ArgumentException($"There was an error during the import: Poll Name exceeds the maximum length of 100 characters.");

                CreateCommandResponse<CreatedPollDTO> createdPoll = await _pollOrchestratorService.ImportPollInstancesAsync(PollsDtos, EvaluationId);
                
                if (createdPoll.Entity == null)
                {
                    _logger.LogError("Error saving data: createdPoll is null");
                    throw new Exception($"Error saving data: createdPoll is null");
                }
                return createdPoll.Entity;
            }
            catch (ArgumentException)
            {
                throw;
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
            string path = $"{ApiUrl}{PathEvaluation}?$top=1000&$filter=name eq '{EvaluationSetName}'";

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
                var pollMap = new Dictionary<int, DataItem>();
                var pollIndex = 0;
                var populatedComponentsTasks = new List<Task<List<ComponentDTO>>>();

                foreach (DataItem responseToPollInstance in apiResponse.data)
                {
                    pollMap.Add(pollIndex, responseToPollInstance);
                    populatedComponentsTasks.Add(PopulateListOfComponentsByIdPollInstanceAsync(
                        componentsAndVariables,
                        responseToPollInstance.Id,
                        responseToPollInstance.score,
                        decryptedApiKey,
                        ApiUrl,
                        StartDate,
                        EndDate
                    ));
                    pollIndex++;
                }

                List<ComponentDTO>[] populatedComponentsList = await Task.WhenAll<List<ComponentDTO>>(populatedComponentsTasks);
                var populatedComponentIndex = 0;

                var alreadyImportedStudentsEmails = await _pollInstanceRepository.GetImportedStudentsEmailsByPollName(EvaluationSetName);

                foreach (List<ComponentDTO> populatedComponent in populatedComponentsList)
                {
                    DataItem responseToPollInstance = pollMap[populatedComponentIndex];
                    if (populatedComponent.Count > 0)
                    {
                        var pollDto = new PollDTO
                        {
                            IdCosmicLatte = responseToPollInstance.Id ?? string.Empty,
                            Uuid = Guid.NewGuid().ToString(),
                            Name = SqlInjectionValidator.Sanitize(responseToPollInstance.name),
                            FinishedAt = responseToPollInstance.finishedAt,
                            LastVersion = 1,
                            LastVersionDate = DateTime.UtcNow,
                            Components = SanitizeComponents(populatedComponent),
                            ParentId = responseToPollInstance.parent.Split(':')[1]
                        };
                        
                        var studentEmail = pollDto.Components?
                            .FirstOrDefault()?.Variables?
                            .FirstOrDefault()?.Answer?.Student?.Email;


                        if (!string.IsNullOrEmpty(studentEmail))
                        {
                            pollDto.IsAlreadyImported = alreadyImportedStudentsEmails.Any(e => e == studentEmail);
                        }
                        pollsDtos.Add(pollDto);
                    }

                    populatedComponentIndex++;
                }
            }

            return pollsDtos;
        }


        public async Task<ExtractionSummary> ExtractRespondentsAsync(
            string EvaluationSetName,
            string StartDate,
            string EndDate,
            string PollId,
            string ApiKey,
            string ApiUrl,
            Func<PollDTO, bool, Task> OnExtracted)
        {
            var decryptedApiKey = _encryptor.Decrypt(ApiKey);
            string path = $"{ApiUrl}{PathEvaluation}?$top=1000&$filter=name eq '{EvaluationSetName}' or parent eq 'evaluationSets:{PollId}'";

            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add(HeaderApiKey, decryptedApiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Cosmic latte server error, Message: {response.ReasonPhrase}");

            string responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody)
                              ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

            int returned = apiResponse.data.Count;
            var validatedEvaluations = apiResponse.data.Where(E => E.status == "validated").ToList();
            if (validatedEvaluations.Count == 0 || validatedEvaluations[0].Id == null)
            {
                _logger.LogWarning("Cosmic Latte returned {Returned} responses for '{EvaluationSetName}' but none is validated; nothing to extract.", returned, EvaluationSetName);
                return ExtractionSummary.Empty with { Returned = returned };
            }

            var variablesPositionByComponents = GetListOfVariablePositionByComponents(validatedEvaluations[0]);
            var componentsAndVariables = await GetComponentsAndVariablesAsync(
                validatedEvaluations[0].Id!, variablesPositionByComponents, decryptedApiKey, ApiUrl);
            if (componentsAndVariables.Count == 0)
            {
                _logger.LogWarning("Could not read the questions of '{EvaluationSetName}' from Cosmic Latte; nothing to extract.", EvaluationSetName);
                return ExtractionSummary.Empty with { Returned = returned };
            }

            var alreadyImportedEmails = await _pollInstanceRepository.GetImportedStudentsEmailsByPollName(EvaluationSetName);

            // Fetch respondent details in parallel (the slow part) but serialize the OnExtracted
            // callback so the caller's scoped DbContext is never used concurrently.
            using var httpGate = new SemaphoreSlim(8);
            using var persistGate = new SemaphoreSlim(1, 1);
            int extracted = 0, withoutScore = 0, requestFailed = 0, outsideRange = 0, invalidAnswers = 0;

            var tasks = apiResponse.data.Select(async dataItem =>
            {
                await httpGate.WaitAsync();
                RespondentFetch fetched;
                try
                {
                    fetched = await FetchRespondentAsync(
                        componentsAndVariables, dataItem.Id, dataItem.score, decryptedApiKey, ApiUrl, StartDate, EndDate);
                }
                finally
                {
                    httpGate.Release();
                }

                List<ComponentDTO> populated = fetched.Components;
                if (populated.Count == 0)
                {
                    switch (fetched.SkipReason)
                    {
                        case RespondentSkipReason.WithoutScore: Interlocked.Increment(ref withoutScore); break;
                        case RespondentSkipReason.RequestFailed: Interlocked.Increment(ref requestFailed); break;
                        case RespondentSkipReason.OutsideDateRange: Interlocked.Increment(ref outsideRange); break;
                        default: Interlocked.Increment(ref invalidAnswers); break;
                    }
                    return;
                }

                var pollDto = new PollDTO
                {
                    IdCosmicLatte = dataItem.Id ?? string.Empty,
                    Uuid = Guid.NewGuid().ToString(),
                    Name = SqlInjectionValidator.Sanitize(dataItem.name),
                    FinishedAt = dataItem.finishedAt,
                    LastVersion = 1,
                    LastVersionDate = DateTime.UtcNow,
                    Components = SanitizeComponents(populated),
                    ParentId = dataItem.parent.Split(':')[1]
                };

                var studentEmail = pollDto.Components?.FirstOrDefault()?.Variables?.FirstOrDefault()?.Answer?.Student?.Email;
                bool alreadyImported = !string.IsNullOrEmpty(studentEmail) && alreadyImportedEmails.Any(E => E == studentEmail);

                await persistGate.WaitAsync();
                try
                {
                    await OnExtracted(pollDto, alreadyImported);
                    Interlocked.Increment(ref extracted);
                }
                finally
                {
                    persistGate.Release();
                }
            });

            await Task.WhenAll(tasks);

            var summary = new ExtractionSummary(returned, extracted, withoutScore, requestFailed, outsideRange, invalidAnswers);
            if (summary.Skipped > 0)
                _logger.LogWarning("{Summary} Evaluation set '{EvaluationSetName}'.", summary, EvaluationSetName);
            else
                _logger.LogInformation("{Summary} Evaluation set '{EvaluationSetName}'.", summary, EvaluationSetName);

            return summary;
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
            RespondentFetch fetched = await FetchRespondentAsync(Components, PollId, ScoreItem, ApiKey, ApiUrl, StartDate, EndDate);
            return fetched.Components;
        }

        private enum RespondentSkipReason
        {
            None,
            WithoutScore,
            RequestFailed,
            OutsideDateRange,
            InvalidAnswers
        }

        private sealed record RespondentFetch(List<ComponentDTO> Components, RespondentSkipReason SkipReason);

        private async Task<RespondentFetch> FetchRespondentAsync(
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
                return new RespondentFetch([], RespondentSkipReason.WithoutScore);
            }

            string path = $"{ApiUrl}{PathEvaluation}/exec/evaluationDetails";
            string requestBody = $"{{\"@data\":{{\"_id\":\"{PollId}\"}}}}";

            string responseBody;
            try
            {
                responseBody = await SendRespondentRequestAsync(path, requestBody, ApiKey);
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                return new RespondentFetch([], RespondentSkipReason.RequestFailed);
            }

            try
            {
                var apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody)
                                  ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");

                string studentName = apiResponse.Data.Answers.ElementAt(0).Value.AnswersList[0];
                string studentEmail = apiResponse.Data.Answers.ElementAt(1).Value.AnswersList[0];
                string studentCohort = apiResponse.Data.Answers.ElementAt(2).Value.AnswersList[0];
                DateTime evaluationFinishedAtDate = apiResponse.Data.Evaluation.FinishedAt;
                StudentDTO studentDto = CreateStudent(studentName, studentEmail, studentCohort);
                List<ComponentDTO> clonedListComponents = new List<ComponentDTO>();
                bool isEvaluationWithinRange = ValidateEvaluationWithinDateRange(StartDate, EndDate, evaluationFinishedAtDate);

                if (isEvaluationWithinRange)
                {
                    clonedListComponents = CloneComponentsList(Components);
                }

                ILookup<int, VariableDTO> variablesByPosition = clonedListComponents
                    .SelectMany(Component => Component.Variables)
                    .ToLookup(Variable => Variable.Position);

                foreach (var answerCL in apiResponse.Data.Answers)
                {
                    foreach (VariableDTO variable in variablesByPosition[answerCL.Value.Position])
                    {
                        variable.Answer = CreateAnswer(answerCL, studentDto, ScoreItem);
                    }
                }

                if (clonedListComponents.Count > 0)
                    return new RespondentFetch(clonedListComponents, RespondentSkipReason.None);

                return new RespondentFetch(
                    clonedListComponents,
                    isEvaluationWithinRange ? RespondentSkipReason.InvalidAnswers : RespondentSkipReason.OutsideDateRange);
            }
            catch (Exception e)
            {
                _logger.LogError($"Cosmic latte server error: {e.Message}");
                return new RespondentFetch([], RespondentSkipReason.InvalidAnswers);
            }
        }

        private async Task<string> SendRespondentRequestAsync(string Path, string Body, string ApiKey)
        {
            for (int attempt = 0; ; attempt++)
            {
                bool canRetry = attempt < RespondentRetryDelays.Length;
                HttpResponseMessage response;
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, Path)
                    {
                        Content = new StringContent(Body, Encoding.UTF8, "application/json")
                    };
                    request.Headers.Add(HeaderApiKey, ApiKey);
                    response = await _httpClient.SendAsync(request);
                }
                catch (HttpRequestException) when (canRetry)
                {
                    await Task.Delay(RespondentRetryDelays[attempt]);
                    continue;
                }

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();

                if (canRetry && IsTransient(response.StatusCode))
                {
                    await Task.Delay(RespondentRetryDelays[attempt]);
                    continue;
                }

                throw new HttpRequestException($"Unsuccessful response from cosmic latte ({(int)response.StatusCode})");
            }
        }

        private static bool IsTransient(HttpStatusCode Status) =>
            Status == HttpStatusCode.TooManyRequests || (int)Status >= 500;

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
                string path = $"{ApiUrl}{PathEvaluation}/exec/evaluationDetails";
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
                string path = BaseUrl + PathEvaluationSet + "?$top=100";
                var request = new HttpRequestMessage(HttpMethod.Get, path);
                request.Headers.Add(HeaderApiKey, decryptedApiKey);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new List<PollDataItem>();

                string responseBody = await response.Content.ReadAsStringAsync();
                CLResponseForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

                List<PollDataItem> pollsData = [.. apiResponse.data.Select(Poll => new PollDataItem(Poll.id, Poll.parent, Poll.name, Poll.status))];
                return pollsData;
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"Invalid Cosmic Latte poll, not supported for this version. {e.Message}");
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

        private bool ValidateEvaluationWithinDateRange(string StartDate, string EndDate, DateTime EvaluationFinishedAtDate)
        {
            bool wasStartDateProvided = !string.IsNullOrEmpty(StartDate);
            bool wasEndDateProvided = !string.IsNullOrEmpty(EndDate);

            if(!wasStartDateProvided && !wasEndDateProvided)
            {
                return true;
            }
            
            // Add 1 day to EndDate to account for evaluations finished
            // on the EndDate.
            if(wasStartDateProvided && wasEndDateProvided && 
               EvaluationFinishedAtDate >= DateTime.Parse(StartDate).ToUniversalTime() &&
               EvaluationFinishedAtDate <= DateTime.Parse(EndDate).ToUniversalTime().AddDays(1)
              )
            {
                return true;
            }

            if(wasStartDateProvided && !wasEndDateProvided &&
               EvaluationFinishedAtDate >= DateTime.Parse(StartDate).ToUniversalTime()
              )
            {
                return true;
            }

            return false;
        }
    }
}
