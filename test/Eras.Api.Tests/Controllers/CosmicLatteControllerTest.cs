using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Api.Controllers;
using Eras.Application.Dtos;
using Eras.Application.Services;
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
            mockService.Setup(service => service.ImportAllPolls(It.Is<string>(name => name == "Encuesta"), It.IsAny<string>(), 
                It.IsAny<string>())).ReturnsAsync(new List<PollDTO>([new PollDTO()]));
            mockService.Setup(service => service.ImportAllPolls(It.Is<string>(name => name != "Encuesta"), It.IsAny<string>(),
                It.IsAny<string>())).ReturnsAsync([]);
            controller = new CosmicLatteController(mockService.Object);
        }
        [Fact]
        public async void ImportPoll_Should_Return_Array ()
        {
            var result = await controller.GetPreviewPolls("Encuesta");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var polls = okResult.Value as List<PollDTO>;
            Assert.NotNull(polls);
            Assert.True(polls.Count > 0);
        }
        [Fact]
        public async void ImportPoll_Should_Return_Empty()
        {
            var result = await controller.GetPreviewPolls("Encuest");
            var okResult = Assert.IsType<ObjectResult>(result);
            var polls = okResult.Value as List<PollDTO>;
            Assert.Null(polls);
        }
    }
}
