using Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Presentation.Controllers;
using Services;

namespace ERAS.Presentation.Tests
{
    public class SampleTest
    {
        [Fact]
        public async Task CosmicApiIsHealthy_ShouldReturnOkResultWithExpectedStatus()
        {
            // Arrange
            var mockService = new Mock<ICosmicLatteAPIService<CosmicLatteStatus>>();
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
