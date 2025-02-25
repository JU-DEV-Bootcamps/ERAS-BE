
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.HeatMap;
using Moq;
using Component = Eras.Domain.Entities.Component;

namespace Eras.Application.Tests.Mocks
{
    public class RepositoryHeatmapMock
    {
        public static Mock<IHeatMapRepository> GetHeatmapRepository() { 
            var mockupHeatmapRepository = new Mock<IHeatMapRepository>();

            var mockAnswers = new List<GetHeatMapByComponentsQueryResponse>
            {
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer1", AnswerRiskLevel = 1 }
            };

            mockupHeatmapRepository.Setup(repo => repo.GetHeatMapDataByComponentsAsync(It.IsAny<string>()
                )).ReturnsAsync(mockAnswers);

            return mockupHeatmapRepository;
        }

        public static Mock<IComponentRepository> GetComponentRepository() { 
            var mockupComponentRepository = new Mock<IComponentRepository>();

            var mockComponents = new List<Component>
            {
                new Component { Id = 1, Name = "Component1" }
            };

            mockupComponentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockComponents);

            return mockupComponentRepository;
        }
    }
}
