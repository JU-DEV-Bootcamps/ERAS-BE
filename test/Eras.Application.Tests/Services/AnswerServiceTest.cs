using System.Reflection.Metadata;

using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Answers.Queries;
using Eras.Application.Services;
using Eras.Domain.Entities;

using Moq;

namespace Eras.Application.Tests.Services;

public class AnswerServiceTest
{
    private readonly Mock<IAnswerRepository> _repository;
    private readonly AnswerService _answerService;

    public AnswerServiceTest() 
    {
        _repository = new Mock<IAnswerRepository>();
        _answerService = new AnswerService(_repository.Object);
    }

    [Fact]
    public async Task Handle_Should_AddAnswerToRepository()
    {
        var answer = new Answer() { 
            AnswerText = "new response",
            PollInstanceId = 1
        };

        _repository
            .Setup(x => x.AddAsync(answer))
            .ReturnsAsync(answer);

        var result = await _answerService.CreateAnswer(answer, null!);

        Assert.NotNull(result);
        Assert.Equal(answer.AnswerText, result.AnswerText);
        Assert.Equal(answer.PollInstanceId, result.PollInstanceId);
    }

    [Fact]
    public async Task Handle_Should_ThrowsException_AsNotImplementedException()
    {
        var query = new Answer();

        _repository
            .Setup(x => x.AddAsync(query))
            .ThrowsAsync(new Exception());

        var exception = await Assert.ThrowsAsync<NotImplementedException>(
            () => _answerService.CreateAnswer(query, null!));

        Assert.Contains("Error creating answer: ", exception.Message);
    }
}
