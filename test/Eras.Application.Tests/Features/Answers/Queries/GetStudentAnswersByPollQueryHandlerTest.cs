using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Answers.Queries;
using Eras.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Eras.Application.Tests.Features.Answers.Queries;
public class GetStudentAnswersByPollQueryHandlerTest
{
    private readonly Mock<IStudentAnswersRepository> _mockAnswerRepository;
    private readonly Mock<ILogger<GetStudentAnswersByPollQueryHandler>> _mockLogger;
    private readonly GetStudentAnswersByPollQueryHandler _handler;

    public GetStudentAnswersByPollQueryHandlerTest()
    {
        _mockAnswerRepository = new Mock<IStudentAnswersRepository>();
        _mockLogger = new Mock<ILogger<GetStudentAnswersByPollQueryHandler>>();
        _handler = new GetStudentAnswersByPollQueryHandler(_mockAnswerRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_ResponseAsync()
    {
        // Arrange
        var query = new GetStudentAnswersByPollQuery() { PollId = 1, StudentId = 1 };
        var studentAnswers = new List<StudentAnswer>() {
            new StudentAnswer
            {
                Answer = "Answer1"
            },
            new StudentAnswer
            {
                Answer = "Answer2"
            }
        };

        _mockAnswerRepository
            .Setup(Repo => Repo.GetStudentAnswersAsync(It.Is<int>(Id => Id == 1),It.Is<int>(Id => Id == 1)))
            .ReturnsAsync(studentAnswers);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }
}
