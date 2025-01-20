using ERAS.Domain.Entities;
using ERAS.Application.Services;
using ERAS.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ERAS.Presentation.Tests
{
    public class PresentationSampleTest
    {
        [Fact]
        public async Task CosmicApiIsHealthy_ShouldReturnOkResultWithExpectedStatus()
        {
            //Arrange

            var mockService = new Mock<ICosmicLatteAPIService>();
            var expectedStatus = new CosmicLatteStatus(true);

            mockService.Setup(service => service.CosmicApiIsHealthy())
                .ReturnsAsync(expectedStatus);


            var controller = new EvaluationsController(mockService.Object);

            // Act
            var result = await controller.CosmicApiIsHealthy();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualStatus = Assert.IsType<CosmicLatteStatus>(okResult.Value);

            Assert.Equal(expectedStatus.Status, actualStatus.Status);
        }
    }
}
