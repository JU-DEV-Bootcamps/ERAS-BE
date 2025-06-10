using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollUuidAndComponent;
using Eras.Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Eras.Application.Tests.Features.Variables.Queries.GetVariablesByPollIdAndComponent
{
    public class GetVariablesByPollUuidAndComponentQueryHandlerTests
    {
        private readonly Mock<IVariableRepository> _mockVariableRepository;
        private readonly Mock<ILogger<GetVariablesByPollUuidAndComponentQueryHandler>> _mockLogger;
        private readonly GetVariablesByPollUuidAndComponentQueryHandler _handler;

        public GetVariablesByPollUuidAndComponentQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetVariablesByPollUuidAndComponentQueryHandler>>();
            _mockVariableRepository = new Mock<IVariableRepository>();
            _handler = new GetVariablesByPollUuidAndComponentQueryHandler(
                _mockVariableRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetVariablesbyPollUuidAndComponentsAsync()
        {
            var polluuid = $"{Guid.NewGuid()}";
            var components = new List<string> { "academico" };
            var lastVersion = true;
            var request = new GetVariablesByPollUuidAndComponentQuery(polluuid, components, lastVersion);

            var variablesData = new List<Variable>
            {
                new Variable { Id = 1, Name = "Cual es tu nombre?" },
                new Variable { Id = 2, Name = "Cual es tu apellido?" },
            };

            _mockVariableRepository
                .Setup(Repo => Repo.GetAllByPollUuidAsync(polluuid, components, lastVersion))
                .ReturnsAsync(variablesData);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<List<Variable>>(result);
        }
    }
}
