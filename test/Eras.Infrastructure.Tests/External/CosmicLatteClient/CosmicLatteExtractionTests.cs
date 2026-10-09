using System.Net;
using System.Text.Json;

using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.Models;
using Eras.Application.Services;
using Eras.Domain.Common;
using Eras.Infrastructure.External.CosmicLatteClient;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.External.CosmicLatteClient;

public class CosmicLatteExtractionTests
{
    private const string ApiUrl = "http://fakeurl.com/";
    private const string StartDate = "2026-01-01";
    private const string EndDate = "2026-01-31";

    private readonly Mock<ILogger<CosmicLatteAPIService>> _logger = new();
    private readonly Mock<IPollInstanceRepository> _pollInstances = new();

    private sealed class RoutingHandler(Func<HttpRequestMessage, string, HttpResponseMessage> Route) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken CancellationToken)
        {
            Calls++;
            string body = Request.Content is null ? string.Empty : await Request.Content.ReadAsStringAsync(CancellationToken);
            return Route(Request, body);
        }
    }

    private CosmicLatteAPIService CreateService(HttpMessageHandler Handler)
    {
        var encryptor = new Mock<IApiKeyEncryptor>();
        encryptor.Setup(E => E.Decrypt(It.IsAny<string>())).Returns((string Value) => Value);
        _pollInstances
            .Setup(R => R.GetImportedStudentsEmailsByPollName(It.IsAny<string>()))
            .ReturnsAsync([]);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(F => F.CreateClient(It.IsAny<string>())).Returns(new HttpClient(Handler));

        var orchestrator = new PollOrchestratorService(
            new Mock<IMediator>().Object,
            new Mock<ILogger<PollOrchestratorService>>().Object,
            new Mock<IEvaluationRepository>().Object,
            _pollInstances.Object);

        return new CosmicLatteAPIService(
            new Mock<IConfiguration>().Object,
            factory.Object,
            _logger.Object,
            orchestrator,
            encryptor.Object,
            _pollInstances.Object);
    }

    private static string Details(string Email, string FinishedAt, bool WithAnswers = true)
    {
        string answers = WithAnswers
            ? $$"""
              "1": { "answer": "Student", "question": { "body": { "es": "Nombre" } }, "position": 1, "score": 0, "type": "openTextSingleline" },
              "2": { "answer": "{{Email}}", "question": { "body": { "es": "Email" } }, "position": 2, "score": 0, "type": "openTextSingleline" },
              "3": { "answer": "Cohort A", "question": { "body": { "es": "Cohorte" } }, "position": 3, "score": 0, "type": "openTextSingleline" },
              "5": { "answer": "Opcion A", "question": { "body": { "es": "Pregunta 5" } }, "position": 5, "score": 0, "type": "multipleChoice" }
              """
            : string.Empty;

        return $$"""
        {
          "@data": {
            "_id": "x",
            "evaluationSet": { "_id": "es1", "name": "Set" },
            "evaluator": { "name": "-", "email": "-" },
            "evaluation": { "_id": "x", "startedAt": "{{FinishedAt}}", "finishedAt": "{{FinishedAt}}", "elapsedTimeInSeconds": 60, "name": "Set" },
            "scores": {},
            "answers": { {{answers}} },
            "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
            "owner": { "email": "o@test.com", "name": "Owner" }
          },
          "@meta": { "@selfLink": "/x" }
        }
        """;
    }

    private static string ListItem(string Id, string FinishedAt, bool WithScore = true)
    {
        string score = WithScore
            ? """
              "score": {
                "byPosition": [ { "score": 1, "position": 5 } ],
                "byTrait": { "academico": { "Sum": 1, "Avg": 1, "Count": 1, "Min": 1, "Max": 1,
                  "Facets": { "academico": { "Sum": 1, "Avg": 1, "Count": 1, "Scores": [ { "Score": 1, "Position": 5 } ] } } } }
              }
              """
            : "\"score\": null";

        return $$"""
        {
          "_id": "{{Id}}", "name": "Set", "parent": "evaluationSets:p1",
          "configuration": { "grantPublicAccessToScore": false },
          "access": "private", "inventoryKey": "k", "inventoryAccess": "private", "inventoryId": "i", "owner": "o",
          "customFieldsSchema": [], "_tenantName": "t", "changeHistory": [], "customFields": [],
          "status": "validated", "accessToken": "a",
          "startedAt": "{{FinishedAt}}", "elapsedTimeInSeconds": 60, "finishedAt": "{{FinishedAt}}",
          {{score}}
        }
        """;
    }

    private static HttpResponseMessage Json(string Content, HttpStatusCode Status = HttpStatusCode.OK) =>
        new(Status) { Content = new StringContent(Content) };

    private static string RespondentId(string Body) =>
        JsonDocument.Parse(Body).RootElement.GetProperty("@data").GetProperty("_id").GetString()!;

    [Fact]
    public async Task ExtractRespondentsAsync_ReportsHowManyResponsesWereReturnedImportedAndSkippedWithTheReason()
    {
        string list = "{ \"@data\": ["
            + string.Join(",",
                ListItem("r1", "2026-01-10T00:00:00Z"),
                ListItem("r2", "2026-01-10T00:00:00Z", WithScore: false),
                ListItem("r3", "2026-01-10T00:00:00Z"),
                ListItem("r4", "2025-06-01T00:00:00Z"),
                ListItem("r5", "2026-01-10T00:00:00Z"),
                ListItem("r6", "2026-01-12T00:00:00Z"))
            + "], \"@meta\": { \"@totalCount\": 6, \"@count\": 6 } }";

        int r6Attempts = 0;
        var handler = new RoutingHandler((Request, Body) =>
        {
            if (Request.Method == HttpMethod.Get)
                return Json(list);

            return RespondentId(Body) switch
            {
                "r1" => Json(Details("r1@jala.university", "2026-01-10T00:00:00Z")),
                "r3" => Json("error", HttpStatusCode.InternalServerError),
                "r4" => Json(Details("r4@jala.university", "2025-06-01T00:00:00Z")),
                "r5" => Json(Details("r5@jala.university", "2026-01-10T00:00:00Z", WithAnswers: false)),
                "r6" => ++r6Attempts == 1
                    ? Json("slow down", HttpStatusCode.TooManyRequests)
                    : Json(Details("r6@jala.university", "2026-01-12T00:00:00Z")),
                _ => Json("unexpected", HttpStatusCode.NotFound),
            };
        });
        var service = CreateService(handler);
        var extractedEmails = new List<string>();

        ExtractionSummary summary = await service.ExtractRespondentsAsync(
            "Set", StartDate, EndDate, "p1", "key", ApiUrl,
            (Poll, AlreadyImported) =>
            {
                extractedEmails.Add(Poll.Components!.First().Variables.First().Answer!.Student!.Email);
                return Task.CompletedTask;
            });

        Assert.Equal(6, summary.Returned);
        Assert.Equal(2, summary.Extracted);
        Assert.Equal(1, summary.SkippedWithoutScore);
        Assert.Equal(1, summary.SkippedRequestFailed);
        Assert.Equal(1, summary.SkippedOutsideDateRange);
        Assert.Equal(1, summary.SkippedInvalidAnswers);
        Assert.Equal(4, summary.Skipped);
        Assert.Equal(2, r6Attempts);
        Assert.Equivalent(new[] { "r1@jala.university", "r6@jala.university" }, extractedEmails);
    }

    [Fact]
    public async Task ExtractRespondentsAsync_LogsAWarningWithTheSummaryWhenSomethingWasSkipped()
    {
        string list = "{ \"@data\": [" + string.Join(",", ListItem("r1", "2026-01-10T00:00:00Z"), ListItem("r2", "2026-01-10T00:00:00Z", WithScore: false))
            + "], \"@meta\": { \"@totalCount\": 2, \"@count\": 2 } }";
        var handler = new RoutingHandler((Request, Body) =>
            Request.Method == HttpMethod.Get ? Json(list) : Json(Details("r1@jala.university", "2026-01-10T00:00:00Z")));
        var service = CreateService(handler);

        await service.ExtractRespondentsAsync("Set", StartDate, EndDate, "p1", "key", ApiUrl, (Poll, AlreadyImported) => Task.CompletedTask);

        _logger.Verify(
            L => L.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ExtractRespondentsAsync_NoValidatedResponses_ReturnsTheCountAndNothingExtracted()
    {
        var handler = new RoutingHandler((Request, Body) => Json("{ \"@data\": [], \"@meta\": { \"@totalCount\": 0, \"@count\": 0 } }"));
        var service = CreateService(handler);
        int callbacks = 0;

        ExtractionSummary summary = await service.ExtractRespondentsAsync(
            "Set", StartDate, EndDate, "p1", "key", ApiUrl, (Poll, AlreadyImported) => { callbacks++; return Task.CompletedTask; });

        Assert.Equal(0, summary.Returned);
        Assert.Equal(0, summary.Extracted);
        Assert.Equal(0, callbacks);
    }

    [Fact]
    public async Task PopulateList_TransientFailureThenSuccess_IsRetried()
    {
        int attempts = 0;
        var handler = new RoutingHandler((Request, Body) =>
            ++attempts < 3
                ? Json("busy", HttpStatusCode.ServiceUnavailable)
                : Json(Details("a@jala.university", "2026-01-10T00:00:00Z")));
        var service = CreateService(handler);

        var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
            [new ComponentDTO { Name = "academico", Variables = [new VariableDTO { Name = "Pregunta 5", Position = 5 }] }],
            "r1", new Score { byPosition = [], byTrait = new ByTrait() }, "key", ApiUrl, StartDate, EndDate);

        Assert.Single(result);
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task PopulateList_PermanentClientError_IsNotRetried()
    {
        var handler = new RoutingHandler((Request, Body) => Json("not found", HttpStatusCode.NotFound));
        var service = CreateService(handler);

        var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
            [], "r1", new Score { byPosition = [], byTrait = new ByTrait() }, "key", ApiUrl, StartDate, EndDate);

        Assert.Empty(result);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task PopulateList_NetworkErrorsKeepFailing_GivesUpAfterTheRetries()
    {
        var handler = new RoutingHandler((Request, Body) => throw new HttpRequestException("network down"));
        var service = CreateService(handler);

        var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
            [], "r1", new Score { byPosition = [], byTrait = new ByTrait() }, "key", ApiUrl, StartDate, EndDate);

        Assert.Empty(result);
        Assert.Equal(3, handler.Calls);
    }

    [Fact]
    public void ExtractionSummary_AddsUpTheSkippedAndDescribesThem()
    {
        var summary = new ExtractionSummary(72, 69, 1, 1, 0, 1);

        Assert.Equal(3, summary.Skipped);
        Assert.Contains("72 responses", summary.ToString());
        Assert.Contains("69 extracted", summary.ToString());
        Assert.Contains("without score: 1", summary.ToString());
        Assert.Equal(0, ExtractionSummary.Empty.Skipped);
    }
}
