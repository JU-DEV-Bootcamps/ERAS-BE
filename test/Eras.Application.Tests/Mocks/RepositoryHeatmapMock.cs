
using Eras.Application.Contracts.Persistence;
using Eras.Application.Models.HeatMap;

using Moq;

using Component = Eras.Domain.Entities.Component;
using Variable = Eras.Domain.Entities.Variable;

namespace Eras.Application.Tests.Mocks
{
    public class RepositoryHeatmapMock
    {
        public static Mock<IHeatMapRepository> GetHeatmapRepository()
        {
            var mockupHeatmapRepository = new Mock<IHeatMapRepository>();

            var mockAnswers = new List<GetHeatMapByComponentsQueryResponse>
            {
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer1", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer2", AnswerRiskLevel = 2 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 2, VariableName = "Variable2", AnswerText = "Answer3", AnswerRiskLevel = 3 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 2, VariableName = "Variable2", AnswerText = "Answer4", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 3, VariableName = "Variable3", AnswerText = "Answer5", AnswerRiskLevel = 2 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 3, VariableName = "Variable3", AnswerText = "Answer6", AnswerRiskLevel = 3 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 4, VariableName = "Variable4", AnswerText = "Answer7", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 4, VariableName = "Variable4", AnswerText = "Answer8", AnswerRiskLevel = 2 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer9", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 3, VariableName = "Variable3", AnswerText = "Answer10", AnswerRiskLevel = 2 },
                // Second repeated rows
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer9", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 3, VariableName = "Variable3", AnswerText = "Answer10", AnswerRiskLevel = 2 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 1, ComponentName = "Component1", VariableId = 1, VariableName = "Variable1", AnswerText = "Answer9", AnswerRiskLevel = 1 },
                new GetHeatMapByComponentsQueryResponse { ComponentId = 2, ComponentName = "Component2", VariableId = 3, VariableName = "Variable3", AnswerText = "Answer10", AnswerRiskLevel = 2 }
            };

            mockupHeatmapRepository.Setup(repo => repo.GetHeatMapDataByComponentsAsync(It.IsAny<string>()
                )).ReturnsAsync(mockAnswers);

            return mockupHeatmapRepository;
        }

        public static Mock<IComponentRepository> GetComponentRepository()
        {
            var mockupComponentRepository = new Mock<IComponentRepository>();

            var mockComponents = new List<Component>
            {
                new Component { Id = 1, Name = "Component1", Variables = new List<Variable> { new Variable { Id = 1, Name = "Variable1" }, new Variable { Id = 2, Name = "Variable2" } } },
                new Component { Id = 2, Name = "Component2", Variables = new List<Variable> { new Variable { Id = 3, Name = "Variable3" }, new Variable { Id = 4, Name = "Variable4" } } }
            };

            mockupComponentRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockComponents);

            return mockupComponentRepository;
        }
    }
}
