using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Variables.Queries.GetVariablesByPollId;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Variables.Queries.GetVariblesByPollId
{
    public class GetVariablesByPollIdQueryHandlerTests
    {
        private readonly Mock<IVariableRepository> _mockVariableRepository;
        private readonly Mock<ILogger<GetVariablesByPollIdQueryHandler>> _mockLogger;
        private readonly GetVariablesByPollIdQueryHandler _handler;

        public GetVariablesByPollIdQueryHandlerTests()
        {
            _mockLogger = new Mock<ILogger<GetVariablesByPollIdQueryHandler>>();
            _mockVariableRepository = new Mock<IVariableRepository>();
            _handler = new GetVariablesByPollIdQueryHandler(
                _mockVariableRepository.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetVariablesbyPollUuidAndComponents()
        {
            var polluuid = $"{Guid.NewGuid()}";
            var components = new List<string> { "academico" };
            var request = new GetVariablesByPollIdAndComponentQuery(polluuid, components);

            var variablesData = new List<Variable>
            {
                new Variable { Id = 1, Name = "Cual es tu nombre?" },
                new Variable { Id = 2, Name = "Cual es tu apellido?" },
            };

            _mockVariableRepository
                .Setup(repo => repo.GetAllByPollUuidAsync(polluuid, components))
                .ReturnsAsync(variablesData);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.IsType<List<Variable>>(result);
        }
    }
}
