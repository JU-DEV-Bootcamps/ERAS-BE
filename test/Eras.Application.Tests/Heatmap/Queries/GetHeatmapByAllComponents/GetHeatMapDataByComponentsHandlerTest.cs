using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Exceptions;
using Eras.Application.Features.HeatMap.Queries.GetHeatMapDataByAllComponents;
using Eras.Application.Tests.Mocks;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Heatmap.Queries.GetHeatmapByAllComponents
{
    public class GetHeatMapDataByComponentsHandlerTest
    {
        private readonly Mock<IHeatMapRepository> _mockHeatmapRepository;
        private readonly Mock<IComponentRepository> _mockComponentRepository;
        private readonly Mock<ILogger<GetHeatMapDataByAllComponentsHandler>> _mockLogger;
        private readonly GetHeatMapDataByAllComponentsHandler _handler;
        public GetHeatMapDataByComponentsHandlerTest()
        {
            _mockHeatmapRepository = RepositoryHeatmapMock.GetHeatmapRepository();
            _mockComponentRepository = RepositoryHeatmapMock.GetComponentRepository();
            _mockLogger = new Mock<ILogger<GetHeatMapDataByAllComponentsHandler>>();
            _handler = new GetHeatMapDataByAllComponentsHandler(_mockHeatmapRepository.Object, _mockComponentRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ShouldThrowNotFoundException_WhenPollInstanceUUIDIsNull()
        {
            
            var request = new GetHeatMapDataByAllComponentsQuery(null);
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldReturnSuccessResponse_WhenDataIsRetrievedSuccessfully() {
            var request = new GetHeatMapDataByAllComponentsQuery("valid-uuid");

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.Success);
            Assert.Equal("Success", result.Message);
            Assert.NotEmpty(result.Body);
            // Validate structure of result.Body 
            Assert.Equal(2, result.Body.Count());
            foreach (var item in result.Body)
            {
                Console.WriteLine($"ComponentName: {item.ComponentName}");
                Assert.Equal(2, item.Variables.Variables.Count());
                Assert.Equal(2, item.Series.Count());
            }
        }

        [Fact]
        public async Task ShouldReturnFailedResponse_WhenExceptionIsThrown()
        {
            var request = new GetHeatMapDataByAllComponentsQuery("valid-uuid");

            _mockHeatmapRepository.Setup(repo => repo.GetHeatMapDataByComponentsAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Failed", result.Message);
            Assert.Empty(result.Body);
        }
    }
}
