using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;

using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace Eras.Infrastructure.External.CosmicLatteClient;

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
        var path = _apiUrl + PathEvalautionSet;
        var request = new HttpRequestMessage(HttpMethod.Get, path + "?$filter=contains(name,' ')");
        request.Headers.Add(HeaderApiKey, _apiKey);

        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request);
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
            CreateCommandResponse<CreatedPollDTO> createdPoll = await _pollOrchestratorService.ImportPollInstances(PollsDtos);
            return createdPoll.Entity;
        }
        catch (Exception e)
        {
            throw new Exception($"Error saving data: {e.Message}");
        }
    }

    public async Task<List<PollDTO>> GetAllPollsPreview(string Name, string StartDate, string EndDate)
    {
        string path = _apiUrl + PathEvalaution;
        if (Name != "" || StartDate != "" || EndDate != "")
        {
            path += "?$filter=";
            if (Name != "") path += $"contains(Name,'{Name}')";
            if (StartDate != "") path += $" and startedAt ge {ConvertStringToIsoExtendedDate(StartDate)}";
            if (EndDate != "") path += $" and startedAt le {ConvertStringToIsoExtendedDate(EndDate)}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Add(HeaderApiKey, _apiKey);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) throw new Exception($"Cosmic latte server error, Message: {response.ReasonPhrase}");

        List<PollDTO> pollsDtos = new List<PollDTO>();

        string responseBody = await response.Content.ReadAsStringAsync();
        CLResponseModelForAllPollsDTO apiResponse;
        try
        {
            apiResponse = JsonSerializer.Deserialize<CLResponseModelForAllPollsDTO>(responseBody)
                ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");
        }
        catch (Exception ex)
        {
            throw new InvalidCastException("Unable to deserialize response from cosmic latte", ex);
        }

        Dictionary<string, List<int>> variablesPositionByComponents = GetListOfVariablePositionByComponents(apiResponse.data[0]);

        // 1. Create components and variables
        List<ComponentDTO> componentsAndVariables = GetComponentsAndVariablesAsync(apiResponse.data[0]._id, variablesPositionByComponents).Result;

        foreach (var responseToPollInstace in apiResponse.data)
        {
            if (responseToPollInstace.status == "validated")
            {
                ICollection<ComponentDTO> populatedComponents = await PopulateListOfComponentsByIdPollInstanceAsync(componentsAndVariables, responseToPollInstace._id, responseToPollInstace.score);

                if (populatedComponents != null)
                {
                    // 2. Create polls
                    string version = responseToPollInstace.parent + "-" + responseToPollInstace.changeHistory.Last().when;

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
        return pollsDtos;
    }
    public async Task<List<ComponentDTO>> PopulateListOfComponentsByIdPollInstanceAsync(List<ComponentDTO> Components, string PollId, Score ScoreItem)
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

            CLResponseModelForPollDTO apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<CLResponseModelForPollDTO>(responseBody)
                              ?? throw new InvalidCastException("Unable to deserialize response from cosmic latte");
            }
            catch (Exception ex)
            {
                throw new InvalidCastException("Unable to deserialize response from cosmic latte", ex);
            }

            var studentName = apiResponse.Data?.Answers?.ElementAtOrDefault(0).Value?.AnswersList?.FirstOrDefault()
                               ?? throw new InvalidOperationException("Student name is missing in the response.");
            var studentEmail = apiResponse.Data?.Answers?.ElementAtOrDefault(1).Value?.AnswersList?.FirstOrDefault() ?? "Not found";
            var studentCohort = apiResponse.Data?.Answers?.ElementAtOrDefault(2).Value?.AnswersList?.FirstOrDefault() ?? "Not found";
            StudentDTO studentDto = CreateStudent(studentName, studentEmail, studentCohort);

            // clone list
            List<ComponentDTO> clonedListComponents = [.. Components.Select(Comp => new ComponentDTO
            {
                Name = Comp.Name,
                Variables = [.. Comp.Variables.Select(Var => new VariableDTO
                {
                    Name = Var.Name,
                    Position = Var.Position,
                    Type = Var.Type,
                    Answer = new AnswerDTO(),
                    Audit = Var.Audit,
                })],
                Audit = Comp.Audit,
            })];

            if (apiResponse?.Data?.Answers == null)
            {
                throw new InvalidOperationException("Answers data is missing in the response.");
            }

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
            _logger.LogError(e, "Cosmic latte server error");
            return [];
        }
    }
    public Dictionary<string, List<int>> GetListOfVariablePositionByComponents(DataItem ClDataItem)
    {
        try
        {
            Dictionary<string, JsonElement>? traits = ClDataItem?.score?.byTrait?.traits;
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
                ICollection<VariableDTO> createdVariables = [];
                if (apiResponse.Data?.Answers == null)
                {
                    throw new InvalidOperationException("Answers data is missing in the response.");
                }
                Dictionary<int, Answers> AnswersList = apiResponse.Data.Answers;
                foreach (KeyValuePair<int, Answers> itemVariable in AnswersList)
                {
                    if (item.Value.Contains(itemVariable.Value.Position))
                    {
                        var newVariable = new VariableDTO
                        {
                            Name = itemVariable.Value.Question.Body["es"],
                            Position = itemVariable.Value.Position,
                            Type = itemVariable.Value.Type,
                            Answer = null
                        };
                        createdVariables.Add(newVariable);
                    }
                }
                // 4. Create components
                var component = new ComponentDTO { Name = item.Key, Variables = createdVariables };
                results.Add(component);
            }
            return results;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cosmic latte server error");
            return [];
        }

    }
    public StudentDTO CreateStudent(string Name, string Email, string Cohort)
    {
        var studentDTO = new StudentDTO { Name = Name, Email = Email, Uuid = null };
        var cohortDTO = new CohortDTO() { Name = Cohort };
        studentDTO.Cohort = cohortDTO;
        return studentDTO;
    }
    public AnswerDTO CreateAnswer(KeyValuePair<int, Answers> AnswersKVPair, StudentDTO Student, Score ScoreItem)
    {
        StringBuilder answerSB = new StringBuilder();
        foreach (var answers in AnswersKVPair.Value.AnswersList)
        {
            answerSB.Append(answers);
        }
        double score = GetScoreByPositionAndAnswer(AnswersKVPair.Value.Position, ScoreItem);
        return new AnswerDTO { Answer = answerSB.ToString(), Score = score, Student = Student };
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
            var path = _apiUrl + PathEvalautionSet;
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add(HeaderApiKey, _apiKey);

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return [];

            var responseBody = await response.Content.ReadAsStringAsync();
            CLResponseForAllPollsDTO apiResponse = JsonSerializer.Deserialize<CLResponseForAllPollsDTO>(responseBody) ?? throw new Exception("Unable to deserialize response from cosmic latte");

            List<PollDataItem> pollsData = apiResponse.data != null
                ? [.. apiResponse.data.Select(Poll => new PollDataItem(Poll.parent, Poll.name, Poll.status))]
                : [];
            return pollsData;
        }
        catch (Exception e)
        {
            throw new InvalidCastException($"Invalid Cosmic Latte poll, not supported for this version. {e.Message}");
        }
    }
}
