using Eras.Api.Controllers;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
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

            controller = new CosmicLatteController(mockMediator.Object, mockService.Object);
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
    }
}
