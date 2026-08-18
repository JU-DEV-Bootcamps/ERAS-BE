using System.Net;
using System.Text.Json;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CL;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using Eras.Infrastructure.External.CosmicLatteClient;
using Eras.Application.Contracts.Persistence; 

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Infrastructure.Tests.External.CosmicLatteClient
{
    public class CosmicLatteAPIServiceTests
    {
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<ILogger<CosmicLatteAPIService>> _loggerMock = new();
        private readonly Mock<IApiKeyEncryptor> _encryptorMock = new();
        private readonly Mock<IPollInstanceRepository> _pollInstanceRepoMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<ILogger<PollOrchestratorService>> _orchestratorLoggerMock = new();
        private readonly Mock<IEvaluationRepository> _evaluationRepoMock = new();

        private CosmicLatteAPIService CreateService(HttpResponseMessage? response = null, string requestUrl = "http://fakeurl.com/")
        {
            _encryptorMock.Setup(e => e.Decrypt(It.IsAny<string>())).Returns((string s) => s); // pass-through

            var httpClient = response != null
                ? new HttpClient(new MockHttpMessageHandler(new Dictionary<string, HttpResponseMessage> { { requestUrl, response } }))
                : new HttpClient(new ThrowingHandler());

            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            factoryMock.Setup(f => f.CreateClient(string.Empty)).Returns(httpClient);

            // PollOrchestratorService es una clase concreta con métodos no-virtuales:
            // no se puede mockear con Moq. La instanciamos real y controlamos su
            // comportamiento a través de sus propias dependencias mockeadas.
            var orchestrator = new PollOrchestratorService(
                _mediatorMock.Object,
                _orchestratorLoggerMock.Object,
                _evaluationRepoMock.Object,
                _pollInstanceRepoMock.Object);

            return new CosmicLatteAPIService(
                _configMock.Object,
                factoryMock.Object,
                _loggerMock.Object,
                orchestrator,
                _encryptorMock.Object,
                _pollInstanceRepoMock.Object);
        }

        private class ThrowingHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => throw new HttpRequestException("network failure");
        }

        // ---------- CosmicApiIsHealthy ----------

        [Fact]
        public async Task CosmicApiIsHealthy_SuccessResponse_ReturnsTrue()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") };
            var service = CreateService(response, "http://fakeurl.com/evaluationSets?$filter=contains(name,' ')");

            var result = await service.CosmicApiIsHealthy("key", "http://fakeurl.com/");

            Assert.True(result.Status);
        }

        [Fact]
        public async Task CosmicApiIsHealthy_HttpThrows_ReturnsFalseAndLogsError()
        {
            var service = CreateService(response: null); // usa ThrowingHandler

            var result = await service.CosmicApiIsHealthy("key", "http://fakeurl.com/");

            Assert.False(result.Status);
            _loggerMock.Verify(
                l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        // ---------- SavePreviewPolls ----------

        [Fact]
        public async Task SavePreviewPolls_PollNameTooLong_ThrowsArgumentException()
        {
            var service = CreateService();
            var polls = new List<PollDTO> { new PollDTO { Name = new string('a', 101) } };

            await Assert.ThrowsAsync<ArgumentException>(() => service.SavePreviewPolls(polls, 1));
        }

        

        // ---------- GetListOfVariablePositionByComponents ----------

        [Fact]
        public void GetListOfVariablePositionByComponents_NullTraits_ReturnsEmptyDictionary()
        {
            var service = CreateService();
            var dataItem = new DataItem
            {
                name = "x", parent = "x", configuration = new Configuration(), access = "x",
                inventoryKey = "x", inventoryAccess = "x", inventoryId = "x", owner = "x",
                customFieldsSchema = new List<string>(), TenantName = "x", changeHistory = new List<ChangeHistoryItem>(),
                customFields = new List<string>(), status = "x", accessToken = "x", score = null
            };

            var result = service.GetListOfVariablePositionByComponents(dataItem);

            Assert.Empty(result);
        }

        [Fact]
        public void GetListOfVariablePositionByComponents_ValidTraits_ReturnsPositionsByComponent()
        {
            var service = CreateService();
            var traitsJson = """
            {
              "academico": {
                "sum": 10, "avg": 5, "count": 2, "min": 1, "max": 9,
                "facets": {
                  "academico": { "sum": 10, "avg": 5, "count": 2,
                    "scores": [{ "score": 1, "position": 5 }, { "score": 9, "position": 6 }] }
                }
              }
            }
            """;
            var traits = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(traitsJson)!;

            var dataItem = new DataItem
            {
                name = "x", parent = "x", configuration = new Configuration(), access = "x",
                inventoryKey = "x", inventoryAccess = "x", inventoryId = "x", owner = "x",
                customFieldsSchema = new List<string>(), TenantName = "x", changeHistory = new List<ChangeHistoryItem>(),
                customFields = new List<string>(), status = "x", accessToken = "x",
                score = new Score { byPosition = new List<ByPosition>(), byTrait = new ByTrait { Traits = traits } }
            };

            var result = service.GetListOfVariablePositionByComponents(dataItem);

            Assert.True(result.ContainsKey("academico"));
            Assert.Equal(new List<int> { 5, 6 }, result["academico"]);
        }

        // ---------- CloneComponentsList ----------

        [Fact]
        public void CloneComponentsList_DeepClonesComponentsAndVariables()
        {
            var original = new List<ComponentDTO>
            {
                new ComponentDTO
                {
                    Name = "Comp1",
                    Variables = new List<VariableDTO>
                    {
                        new VariableDTO { Name = "Var1", Position = 1, Type = "text" }
                    }
                }
            };

            var cloned = CosmicLatteAPIService.CloneComponentsList(original);

            Assert.Equal("Comp1", cloned[0].Name);
            Assert.Equal("Var1", cloned[0].Variables.First().Name);

            // Mutar el clon no debe afectar al original
            cloned[0].Name = "Mutated";
            Assert.Equal("Comp1", original[0].Name);
        }

        // ---------- CreateStudent / CreateAnswer ----------

        [Fact]
        public void CreateStudent_SetsNameEmailAndCohort()
        {
            var service = CreateService();

            var student = service.CreateStudent("Ana", "ana@test.com", "Cohort A");

            Assert.Equal("Ana", student.Name);
            Assert.Equal("ana@test.com", student.Email);
            Assert.Equal("Cohort A", student.Cohort.Name);
        }

        [Fact]
        public void CreateAnswer_OpenEndedQuestion_ScoreIsZero()
        {
            var service = CreateService();
            var student = new StudentDTO();
            var kvp = new KeyValuePair<int, Answers>(1, new Answers
            {
                AnswersList = new[] { "Free text answer" },
                Question = new Question { Body = new Dictionary<string, string> { { "es", "q" } } },
                Position = 1,
                Type = "openTextSingleline"
            });
            var scoreItem = new Score { byPosition = new List<ByPosition>(), byTrait = new ByTrait() };

            var answer = service.CreateAnswer(kvp, student, scoreItem);

            Assert.Equal(0m, answer.Score);
            Assert.Equal("Free text answer", answer.Answer);
        }

        [Fact]
        public void CreateAnswer_MultipleChoice_UsesScoreByPosition()
        {
            var service = CreateService();
            var student = new StudentDTO();
            var kvp = new KeyValuePair<int, Answers>(1, new Answers
            {
                AnswersList = new[] { "Opción A" },
                Question = new Question { Body = new Dictionary<string, string> { { "es", "q" } } },
                Position = 5,
                Type = "multipleChoice"
            });
            var scoreItem = new Score
            {
                byPosition = new List<ByPosition> { new ByPosition { position = 5, score = 8 } },
                byTrait = new ByTrait()
            };

            var answer = service.CreateAnswer(kvp, student, scoreItem);

            Assert.Equal(8m, answer.Score);
        }

        // ---------- GetComponentsAndVariablesAsync ----------

        [Fact]
        public async Task GetComponentsAndVariablesAsync_UnsuccessfulResponse_ReturnsEmptyList()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("error") };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");

            var result = await service.GetComponentsAndVariablesAsync("pollId", new Dictionary<string, List<int>>(), "key", "http://fakeurl.com/");

            Assert.Empty(result);
        }

        // ---------- GetPollsNameList ----------

        [Fact]
        public async Task GetPollsNameList_UnsuccessfulResponse_ReturnsEmptyList()
        {
            var response = new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent("") };
            var service = CreateService(response, "http://fakeurl.com/evaluationSets?$top=100");

            var result = await service.GetPollsNameList("http://fakeurl.com/", "key");

            Assert.Empty(result);
        }

        // ---------- GetComponentsAndVariablesAsync (happy path) ----------

        [Fact]
        public async Task GetComponentsAndVariablesAsync_SuccessResponse_BuildsComponentsWithVariables()
        {
            // Arrange
            var json = """
            {
            "@data": {
                "_id": "poll1",
                "evaluationSet": { "_id": "es1", "name": "Set" },
                "evaluator": { "name": "-", "email": "-" },
                "evaluation": { "_id": "poll1", "startedAt": "2026-01-01T00:00:00Z", "finishedAt": "2026-01-01T00:10:00Z", "elapsedTimeInSeconds": 600, "name": "Encuesta" },
                "scores": {},
                "answers": {
                "5": { "answer": "Opción A", "question": { "body": { "es": "Pregunta 5" } }, "position": 5, "score": 0, "type": "multipleChoice" }
                },
                "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
                "owner": { "email": "o@test.com", "name": "Owner" }
            },
            "@meta": { "@selfLink": "/x" }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");
            var variablesPositionByComponents = new Dictionary<string, List<int>> { { "academico", new List<int> { 5 } } };

            // Act
            var result = await service.GetComponentsAndVariablesAsync("poll1", variablesPositionByComponents, "key", "http://fakeurl.com/");

            // Assert
            Assert.Single(result);
            Assert.Equal("academico", result[0].Name);
            Assert.Single(result[0].Variables);
            Assert.Equal("Pregunta 5", result[0].Variables.First().Name);
            Assert.Equal(5, result[0].Variables.First().Position);
        }

        [Fact]
        public async Task GetComponentsAndVariablesAsync_OpenEndedQuestionOutsideKnownPositions_GoesToPersonalDataAndIsSkipped()
        {
            // Arrange: pregunta abierta (posición 99) que no cae en el rango de ningún componente conocido
            // -> DetermineComponentForOpenEndedQuestion la manda a "personalData", que el código
            // descarta explícitamente (continue). Este test documenta ese comportamiento.
            var json = """
            {
            "@data": {
                "_id": "poll1",
                "evaluationSet": { "_id": "es1", "name": "Set" },
                "evaluator": { "name": "-", "email": "-" },
                "evaluation": { "_id": "poll1", "startedAt": "2026-01-01T00:00:00Z", "finishedAt": "2026-01-01T00:10:00Z", "elapsedTimeInSeconds": 600, "name": "Encuesta" },
                "scores": {},
                "answers": {
                "5": { "answer": "Opción A", "question": { "body": { "es": "Pregunta 5" } }, "position": 5, "score": 0, "type": "multipleChoice" },
                "99": { "answer": "Texto libre", "question": { "body": { "es": "Pregunta abierta" } }, "position": 99, "score": 0, "type": "openTextSingleline" }
                },
                "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
                "owner": { "email": "o@test.com", "name": "Owner" }
            },
            "@meta": { "@selfLink": "/x" }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");
            var variablesPositionByComponents = new Dictionary<string, List<int>> { { "academico", new List<int> { 5 } } };

            // Act
            var result = await service.GetComponentsAndVariablesAsync("poll1", variablesPositionByComponents, "key", "http://fakeurl.com/");

            // Assert: la pregunta abierta NO aparece en ningún componente (quedó en personalData y se descartó)
            Assert.Single(result);
            Assert.Single(result[0].Variables); // solo la de posición 5, no la 99
        }

        [Fact]
        public async Task GetComponentsAndVariablesAsync_OpenEndedQuestionWithinComponentRange_IsAddedToThatComponent()
        {
            // Arrange: pregunta abierta en posición 6, dentro del rango [5,7] del componente "academico"
            var json = """
            {
            "@data": {
                "_id": "poll1",
                "evaluationSet": { "_id": "es1", "name": "Set" },
                "evaluator": { "name": "-", "email": "-" },
                "evaluation": { "_id": "poll1", "startedAt": "2026-01-01T00:00:00Z", "finishedAt": "2026-01-01T00:10:00Z", "elapsedTimeInSeconds": 600, "name": "Encuesta" },
                "scores": {},
                "answers": {
                "5": { "answer": "Opción A", "question": { "body": { "es": "Pregunta 5" } }, "position": 5, "score": 0, "type": "multipleChoice" },
                "7": { "answer": "Opción B", "question": { "body": { "es": "Pregunta 7" } }, "position": 7, "score": 0, "type": "multipleChoice" },
                "6": { "answer": "Texto libre", "question": { "body": { "es": "Pregunta abierta" } }, "position": 6, "score": 0, "type": "openTextSingleline" }
                },
                "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
                "owner": { "email": "o@test.com", "name": "Owner" }
            },
            "@meta": { "@selfLink": "/x" }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");
            var variablesPositionByComponents = new Dictionary<string, List<int>> { { "academico", new List<int> { 5, 7 } } };

            // Act
            var result = await service.GetComponentsAndVariablesAsync("poll1", variablesPositionByComponents, "key", "http://fakeurl.com/");

            // Assert: ahora "academico" tiene 3 variables (5, 7, y la abierta 6 insertada), ordenadas por posición
            Assert.Single(result);
            Assert.Equal(3, result[0].Variables.Count);
            Assert.Equal(new[] { 5, 6, 7 }, result[0].Variables.Select(v => v.Position));
        }

        // ---------- PopulateListOfComponentsByIdPollInstanceAsync (happy path + errores) ----------

        [Fact]
        public async Task PopulateListOfComponentsByIdPollInstanceAsync_SuccessWithinDateRange_PopulatesAnswers()
        {
            // Arrange
            var json = """
            {
            "@data": {
                "_id": "poll1",
                "evaluationSet": { "_id": "es1", "name": "Set" },
                "evaluator": { "name": "-", "email": "-" },
                "evaluation": { "_id": "poll1", "startedAt": "2026-06-01T00:00:00Z", "finishedAt": "2026-06-15T00:00:00Z", "elapsedTimeInSeconds": 600, "name": "Encuesta" },
                "scores": {},
                "answers": {
                "1": { "answer": "Juan Pérez", "question": { "body": { "es": "Nombre" } }, "position": 1, "score": 0, "type": "openTextSingleline" },
                "2": { "answer": "juan@test.com", "question": { "body": { "es": "Email" } }, "position": 2, "score": 0, "type": "openTextSingleline" },
                "3": { "answer": "Cohort A", "question": { "body": { "es": "Cohorte" } }, "position": 3, "score": 0, "type": "openTextSingleline" },
                "5": { "answer": "Nombre", "question": { "body": { "es": "Nombre" } }, "position": 5, "score": 0, "type": "openTextSingleline" }
                },
                "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
                "owner": { "email": "o@test.com", "name": "Owner" }
            },
            "@meta": { "@selfLink": "/x" }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");

            var components = new List<ComponentDTO>
            {
                new ComponentDTO
                {
                    Name = "academico",
                    Variables = new List<VariableDTO> { new VariableDTO { Name = "Nombre", Position = 5, Type = "openTextSingleline" } }
                }
            };
            var scoreItem = new Score { byPosition = new List<ByPosition> { new ByPosition { position = 5, score = 10 } }, byTrait = new ByTrait() };

            // Act: rango de fechas que SI contiene el 2026-06-15
            var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
                components, "poll1", scoreItem, "key", "http://fakeurl.com/", "2026-06-01", "2026-06-30");

            // Assert
            Assert.Single(result);
            var answer = result[0].Variables.First().Answer;
            Assert.NotNull(answer);
            Assert.Equal("Nombre", answer!.Answer);
            Assert.Equal("academico", result[0].Name); // vino de CloneComponentsList, no del original
        }

        [Fact]
        public async Task PopulateListOfComponentsByIdPollInstanceAsync_OutsideDateRange_ReturnsComponentsWithoutClonedAnswers()
        {
            // Arrange: misma respuesta pero pedimos un rango de fechas que NO contiene el finishedAt
            var json = """
            {
            "@data": {
                "_id": "poll1",
                "evaluationSet": { "_id": "es1", "name": "Set" },
                "evaluator": { "name": "-", "email": "-" },
                "evaluation": { "_id": "poll1", "startedAt": "2026-01-01T00:00:00Z", "finishedAt": "2026-01-15T00:00:00Z", "elapsedTimeInSeconds": 600, "name": "Encuesta" },
                "scores": {},
                "answers": {
                "answers": {
                "1": { "answer": "Juan Pérez", "question": { "body": { "es": "Nombre" } }, "position": 1, "score": 0, "type": "openTextSingleline" },
                "2": { "answer": "juan@test.com", "question": { "body": { "es": "Email" } }, "position": 2, "score": 0, "type": "openTextSingleline" },
                "3": { "answer": "Cohort A", "question": { "body": { "es": "Cohorte" } }, "position": 3, "score": 0, "type": "openTextSingleline" },
                "5": { "answer": "Nombre", "question": { "body": { "es": "Nombre" } }, "position": 5, "score": 0, "type": "openTextSingleline" }
                },
                "inventory": { "_id": "inv1", "name": "Inv", "key": "k", "access": "private" },
                "owner": { "email": "o@test.com", "name": "Owner" }
            },
            "@meta": { "@selfLink": "/x" }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");

            var components = new List<ComponentDTO>
            {
                new ComponentDTO { Name = "academico", Variables = new List<VariableDTO> { new VariableDTO { Name = "Nombre", Position = 5 } } }
            };
            var scoreItem = new Score { byPosition = new List<ByPosition>(), byTrait = new ByTrait() };

            // Act: rango de fechas de junio, evaluación es de enero -> fuera de rango
            var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
                components, "poll1", scoreItem, "key", "http://fakeurl.com/", "2026-06-01", "2026-06-30");

            // Assert: clonedListComponents queda vacío porque isEvaluationWithinRange es false
            Assert.Empty(result);
        }

        [Fact]
        public async Task PopulateListOfComponentsByIdPollInstanceAsync_UnsuccessfulResponse_ReturnsEmptyListAndLogsError()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError) { Content = new StringContent("error") };
            var service = CreateService(response, "http://fakeurl.com/evaluations/exec/evaluationDetails");
            var scoreItem = new Score { byPosition = new List<ByPosition>(), byTrait = new ByTrait() };

            // Act
            var result = await service.PopulateListOfComponentsByIdPollInstanceAsync(
                new List<ComponentDTO>(), "poll1", scoreItem, "key", "http://fakeurl.com/", "", "");

            // Assert
            Assert.Empty(result);
            _loggerMock.Verify(
                l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        // ---------- GetPollsNameList (happy path) ----------

        [Fact]
        public async Task GetPollsNameList_SuccessResponse_ReturnsMappedList()
        {
            // Arrange
            var json = """
            {
            "@data": [
                { "id": "p1", "parent": "evaluationSets:es1", "name": "Encuesta A", "status": "validated" },
                { "id": "p2", "parent": "evaluationSets:es1", "name": "Encuesta B", "status": "started" }
            ],
            "@meta": { "@totalCount": 2, "@count": 2 }
            }
            """;
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) };
            var service = CreateService(response, "http://fakeurl.com/evaluationSets?$top=100");

            // Act
            var result = await service.GetPollsNameList("http://fakeurl.com/", "key");

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.name == "Encuesta A");
            
        }

        // ---------- GetListOfVariablePositionByComponents (rama de excepción) ----------

        [Fact]
        public void GetListOfVariablePositionByComponents_MalformedTraits_ThrowsInvalidCastException()
        {
            // Arrange: traits con una forma que rompe la deserialización interna a TraitData
            var service = CreateService();
            var malformedTraitsJson = """{ "academico": "esto no es un objeto valido" }""";
            var traits = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(malformedTraitsJson)!;

            var dataItem = new DataItem
            {
                name = "x", parent = "x", configuration = new Configuration(), access = "x",
                inventoryKey = "x", inventoryAccess = "x", inventoryId = "x", owner = "x",
                customFieldsSchema = new List<string>(), TenantName = "x", changeHistory = new List<ChangeHistoryItem>(),
                customFields = new List<string>(), status = "x", accessToken = "x",
                score = new Score { byPosition = new List<ByPosition>(), byTrait = new ByTrait { Traits = traits } }
            };

            // Act & Assert
            Assert.Throws<InvalidCastException>(() => service.GetListOfVariablePositionByComponents(dataItem));
        }
    }
}