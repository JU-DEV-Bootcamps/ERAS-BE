using Eras.Api.Controllers;
using Eras.Application.Contracts.Services;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.DTOs.CosmicLatte;
using Eras.Application.Features.Configurations.Queries.GetConfiguration;
using Eras.Application.Services;
using Eras.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Eras.Api.Tests.Controllers
{
    public class CosmicLatteControllerTest
    {
        Mock<ICosmicLatteAPIService> mockService = new();
        Mock<IImportJobService> mockImportJobService = new();
        Mock<IFeatureFlagService> mockFeatureFlagService = new();
        private CosmicLatteController controller;

        public CosmicLatteControllerTest()
        {
            mockService = new Mock<ICosmicLatteAPIService>();

            var mockMediator = new Mock<IMediator>();

            mockMediator
                .Setup(m => m.Send(It.IsAny<GetConfigurationQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Configurations
                {
                    EncryptedKey = "fake-key",
                    BaseURL = "https://fake-url.com"
                });

            mockService.Setup(service => service.GetAllPollsPreview(
                It.Is<string>(name => name == "Encuesta"),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
                )).ReturnsAsync(new List<PollDTO> { new PollDTO() });

            mockService.Setup(service => service.GetAllPollsPreview(
                It.Is<string>(name => name == "Name not found"),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
                )).ReturnsAsync(new List<PollDTO>());

            mockImportJobService = new Mock<IImportJobService>();
            mockFeatureFlagService = new Mock<IFeatureFlagService>();

            controller = new CosmicLatteController(
                mockMediator.Object,
                mockService.Object,
                mockImportJobService.Object,
                mockFeatureFlagService.Object);
        }


        [Fact]
        public async void ImportPoll_Should_Return_ArrayAsync()
        {
            var result = await controller.GetPreviewPollsAsync("Encuesta", "2024-01-01", "2024-12-31", 1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var polls = okResult.Value as List<PollDTO>;
            Assert.NotNull(polls);
            Assert.True(polls.Count > 0);
        }
        [Fact]
        public async void ImportPoll_Should_Return_EmptyAsync()
        {
            var result = await controller.GetPreviewPollsAsync("Name not found", "2024-01-01", "2024-12-31", 1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var polls = okResult.Value as List<PollDTO>;
            Assert.Empty(polls ?? []);
        }

        [Fact]
        public async Task GetPreviewPollsAsync_Should_Return_BadRequest_WhenNameIsTooLongAsync()
        {
            var name = new string('A', 101);
            var result = await controller.GetPreviewPollsAsync(name, "", "", 1);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task GetPreviewPollsAsync_Should_Return_BadRequest_ArgumentExceptionAsync()
        {
            mockService
                .Setup(x => x.GetAllPollsPreview(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Invalid configuration"));
            var result = await controller.GetPreviewPollsAsync("Poll", "", "", 1);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPreviewPollsAsync_Should_Return_MaxLengthError_WhenEvaluationNotFoundAsync()
        {
            var name = new string('A', 100);
            mockService
                .Setup(x => x.GetAllPollsPreview(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("Evaluation not found"));
            var result = await controller.GetPreviewPollsAsync(name, "", "", 1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPreviewPollsAsync_Should_Return_BadReques_InvalidCastExceptionAsync()
        {
            mockService
                .Setup(x => x.GetAllPollsPreview(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new InvalidCastException());
            var result = await controller.GetPreviewPollsAsync("Poll", "", "", 1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPreviewPollsAsync_Should_Return_InternalServerErrorAsync()
        {
            mockService
                .Setup(x => x.GetAllPollsPreview(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Something failed"));
            var result = await controller.GetPreviewPollsAsync("Poll", "", "", 1);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task IsCosmicLatteApiHealthyAsync_Should_Return_500Async()
        {
            mockService
                .Setup(x => x.CosmicApiIsHealthy(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error"));
            var result = await controller.IsCosmicLatteApiHealthyAsync(1);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task SavePreviewPollsAsync_Should_Return_OkResultWithFlagAsync()
        {
            mockFeatureFlagService
                .Setup(x => x.UseEnhancedEvaluationImport())
                .ReturnsAsync(true);
            mockImportJobService
                .Setup(x => x.QueueImportAsync(It.IsAny<List<PollDTO>>(), 5))
                .ReturnsAsync(123);
            var result = await controller.SavePreviewPollsAsync(It.IsAny<List<PollDTO>>(), 1);
            var okResult = Assert.IsType<AcceptedResult>(result);
            Assert.IsType<AcceptedResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task SavePreviewPollsAsync_Should_Return_OkResultAWithFlagOffAsync()
        {
            mockFeatureFlagService
                .Setup(x => x.UseEnhancedEvaluationImport())
                .ReturnsAsync(false);
            mockService
                .Setup(x => x.SavePreviewPolls(It.IsAny<List<PollDTO>>(), 5))
                .ReturnsAsync(new CreatedPollDTO { 
                    Id = 1,
                    FinishedAt = DateTime.UtcNow,
                    IdCosmicLatte = "",
                    Uuid = "",
                    Version = "1.0.0",
                    Name = "name",
                    studentDTOs = [],
                });
            var result = await controller.SavePreviewPollsAsync(It.IsAny<List<PollDTO>>(), 1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task StartExtraction_Should_Return_OkResultAsync()
        {
            mockImportJobService
                .Setup(x => x.StartExtractionAsync("", 1, "", "", 5))
                .ReturnsAsync(1);
            var result = await controller.StartExtractionAsync(new StartExtractionRequest
            {
                EvaluationSetName = "",
                ConfigurationId = 1,
                EvaluationId = 5,
            });
            var okResult = Assert.IsType<AcceptedResult>(result);
            Assert.NotNull(result);
            Assert.IsType<AcceptedResult>(result);
        }

        [Fact]
        public async Task StartExtractionAsync_Should_Return_BadRequest_WhenArgumentExceptionAsync()
        {
            var request = new StartExtractionRequest
            {
                EvaluationSetName = "Evaluation",
                ConfigurationId = 1,
                StartDate = "2024-01-01",
                EndDate = "2024-12-31",
                EvaluationId = 5
            };
            mockImportJobService
                .Setup(x => x.StartExtractionAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .ThrowsAsync(new ArgumentException("Error"));
            var result = await controller.StartExtractionAsync(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task ConfirmImport_Should_Return_BadRequest_WhenEmptyRequestAsync()
        {
            var request = new ConfirmImportRequest
            {
                ItemIds = new List<int>()
            };
            var result = await controller.ConfirmImportAsync(1, request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmImport_Should_Return_BadRequest_WhenNotFoundJobAsync()
        {
            mockImportJobService
                .Setup(x => x.ConfirmImportAsync(1, It.IsAny<List<int>>()))
                .ReturnsAsync(false);
            var request = new ConfirmImportRequest{ ItemIds = new List<int>([1])};
            var result = await controller.ConfirmImportAsync(1, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmImport_Should_Return_AcceptedRequestAsync()
        {
            mockImportJobService
                .Setup(x => x.ConfirmImportAsync(1, It.IsAny<List<int>>()))
                .ReturnsAsync(true);
            var request = new ConfirmImportRequest { ItemIds = new List<int>([1]) };
            var result = await controller.ConfirmImportAsync(1, request);
            Assert.IsType<AcceptedResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RetryImportItemsAsync_Should_Return_BadRequest_WhenEmptyRequestAsync()
        {
            var request = new RetryImportItemsRequest
            {
                ItemIds = new List<int>()
            };
            var result = await controller.RetryImportItemsAsync(1, request);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RetryImportItemsAsync_Should_Return_BadRequest_WhenNotFoundJobAsync()
        {
            mockImportJobService
                .Setup(x => x.RetryItemsAsync(1, It.IsAny<List<int>>()))
                .ReturnsAsync(false);
            var request = new RetryImportItemsRequest { ItemIds = new List<int>([1]) };
            var result = await controller.RetryImportItemsAsync(1, request);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task RetryImportItemsAsync_Should_Return_AcceptedRequestAsync()
        {
            mockImportJobService
                .Setup(x => x.RetryItemsAsync(1, It.IsAny<List<int>>()))
                .ReturnsAsync(true);
            var request = new RetryImportItemsRequest { ItemIds = new List<int>([1]) };
            var result = await controller.RetryImportItemsAsync(1, request);
            Assert.IsType<AcceptedResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetImportItemsAsync_Should_Return_BadRequest_WhenNotFoundJobAsync()
        {
            mockImportJobService
                .Setup(x => x.GetStatusAsync(1))
                .ReturnsAsync((ImportJobStatusDTO?)null);
            var result = await controller.GetImportItemsAsync(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetImportItemsAsync_Should_Return_AcceptedRequestAsync()
        {
            mockImportJobService
                .Setup(x => x.GetStatusAsync(1))
                .ReturnsAsync(new ImportJobStatusDTO
                {
                    ImportJobId = 1,
                    EvaluationId = 2,
                    Status = "",
                    TotalCount = 10,
                    ProcessedCount = 2,
                    ExtractedCount = 2,
                    RetryCount = 6,
                    CreatedAtUtc = DateTime.Now,
                    UpdatedAtUtc = DateTime.Now,
                });
            mockImportJobService
               .Setup(x => x.GetItemsAsync(1))
               .ReturnsAsync([new ImportJobItemDTO
               {
                   Id = 1,
                   ImportJobId = 2,
                   StudentEmail = "",
                   StudentName = "",
                   Status = "",
                   RetryCount = 2,
                   IsAlreadyImported = false,
               }]);
            var result = await controller.GetImportItemsAsync(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetImportStatusAsync_Should_Return_BadRequest_WhenNotFoundJobAsync()
        {
            mockImportJobService
                .Setup(x => x.GetStatusAsync(1))
                .ReturnsAsync((ImportJobStatusDTO?)null);
            var result = await controller.GetImportStatusAsync(1);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetImportStatusAsync_Should_Return_AcceptedRequestAsync()
        {
            mockImportJobService
                .Setup(x => x.GetStatusAsync(1))
                .ReturnsAsync( new ImportJobStatusDTO
                {
                    ImportJobId = 1, 
                    EvaluationId = 2,
                    Status = "",
                    TotalCount = 10,
                    ProcessedCount = 2,
                    ExtractedCount = 2,
                    RetryCount = 6,
                    CreatedAtUtc = DateTime.Now,
                    UpdatedAtUtc = DateTime.Now,
                });
            var result = await controller.GetImportStatusAsync(1);
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPollsNameListAsync_Should_Return_BadRequest_ForInvalidCastExceptionAsync()
        {
            mockService
                .Setup(x => x.GetPollsNameList(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new InvalidCastException(""));
            var result = await controller.GetPollsNameListAsync(1);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPollsNameListAsync_Should_Return_BadRequest_WhenExceptionRaisedAsync()
        {
            mockService
                .Setup(x => x.GetPollsNameList(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Error"));
            var result = await controller.GetPollsNameListAsync(1);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetPollsNameListAsync_Should_Return_OkRequestAsync()
        {
            mockService
                .Setup(x => x.GetPollsNameList(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync([new PollDataItem("", "", "","")]);
            var result = await controller.GetPollsNameListAsync(1);
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(result);
        }
    }
}