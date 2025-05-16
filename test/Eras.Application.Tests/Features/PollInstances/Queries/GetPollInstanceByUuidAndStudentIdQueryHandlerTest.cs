using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Features.PollInstances.Queries.GetPollInstancesByCohortAndDays;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.PollInstances.Queries;
public class GetPollInstanceByUuidAndStudentIdQueryHandlerTest
{
    private readonly Mock<IPollInstanceRepository> _mockPollRepository;
    private readonly Mock<ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler>> _mockLogger;
    private readonly GetPollInstanceByUuidAndStudentIdQueryHandler _handler;

    public GetPollInstanceByUuidAndStudentIdQueryHandlerTest()
    {
        _mockPollRepository = new Mock<IPollInstanceRepository>();
        _mockLogger = new Mock<ILogger<GetPollInstanceByUuidAndStudentIdQueryHandler>>();
        _handler = new GetPollInstanceByUuidAndStudentIdQueryHandler(_mockPollRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetPollInstanceByUuidAndStudentIdQuery() { PollUuid = "uuid1", StudentId = 1 };
        var pollInstance = new PollInstance { Id=1,Uuid = "uuid1", FinishedAt = DateTime.UtcNow,
            LastVersionDate = DateTime.Now};
        _mockPollRepository
        .Setup(Repo => Repo.GetByUuidAndStudentIdAsync(It.IsAny<string>(), It.IsAny<int>()))
        .ReturnsAsync(pollInstance);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Poll Found", result.Message);
        Assert.Equal("uuid1", result.Body.Uuid);
    }
}
