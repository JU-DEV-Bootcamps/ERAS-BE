﻿using Eras.Api.Controllers;
using Eras.Application.Dtos;
using Eras.Application.Services;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Eras.Api.Tests.Controllers;

public class CosmicLatteControllerTest
{
    readonly Mock<ICosmicLatteAPIService> mockService = new();
    private readonly CosmicLatteController controller;
    public CosmicLatteControllerTest()
    {
        mockService.Setup(Service => Service.GetAllPollsPreview(It.Is<string>(Name => Name == "Encuesta"), It.IsAny<string>(),
            It.IsAny<string>())).ReturnsAsync(new List<PollDTO>([new PollDTO()]));
        mockService.Setup(Service => Service.GetAllPollsPreview(It.Is<string>(Name => Name == "Name not found"), It.IsAny<string>(),
            It.IsAny<string>())).ReturnsAsync([]);
        controller = new CosmicLatteController(mockService.Object);
    }
    [Fact]
    public async void ImportPoll_Should_Return_ArrayAsync()
    {
        var result = await controller.GetPreviewPolls("Encuesta");
        var okResult = Assert.IsType<OkObjectResult>(result);
        var polls = okResult.Value as List<PollDTO>;
        Assert.NotNull(polls);
        Assert.True(polls.Count > 0);
    }
    [Fact]
    public async void ImportPoll_Should_Return_EmptyAsync()
    {
        IActionResult result = await controller.GetPreviewPolls("Name not found");
        OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
        var polls = okResult.Value as List<PollDTO>;
        Assert.NotNull(polls);
        Assert.Empty(polls);
    }
}
