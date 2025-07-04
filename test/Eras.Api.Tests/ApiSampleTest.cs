using Eras.Api.Controllers;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Api.Tests
{
    public class ApiSampleTest
    {
        [Fact]
        public void CosmicApiIsHealthy_ShouldReturnOkResultWithExpectedStatus()
        {
            //Arrange

            var mockService = new Mock<ICosmicLatteAPIService>();
            var expectedStatus = new CosmicLatteStatus(true);

            mockService.Setup(Service => Service.CosmicApiIsHealthy("",""))
                .ReturnsAsync(expectedStatus);


            // Act
            // var result = await controller.CosmicApiIsHealthy();

            // Assert
            // var okResult = Assert.IsType<OkObjectResult>(result.Result);
            // var actualStatus = Assert.IsType<CosmicLatteStatus>(okResult.Value);

            // Assert.Equal(expectedStatus.Status, actualStatus.Status);
        }
    }
}
