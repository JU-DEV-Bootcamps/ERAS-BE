using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Eras.Application.Contracts.Persistence;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Eras.Domain.Entities;
using Eras.Application.Models.Response.Common;
using Eras.Application.DTOs;
using Eras.Application.Mappers;

namespace Eras.Application.Tests.Features.Answers.Commands;
public class CreateAnswerListCommandHandlerTests
{
    private readonly Mock<IAnswerRepository> _answerRepositoryMock;
    private readonly Mock<ILogger<CreateAnswerListCommandHandler>> _loggerMock;
    private readonly CreateAnswerListCommandHandler _handler;

    public CreateAnswerListCommandHandlerTests()
    {
        _answerRepositoryMock = new Mock<IAnswerRepository>();
        _loggerMock = new Mock<ILogger<CreateAnswerListCommandHandler>>();
        _handler = new CreateAnswerListCommandHandler(_answerRepositoryMock.Object, _loggerMock.Object);
    }

    private static Answer BuildAnswer(int pollInstanceId = 1, int pollVariableId = 1, decimal riskLevel = 1, string answerText = "text")
    {
        var answer = new Answer
        {
            PollInstanceId = pollInstanceId,
            PollVariableId = pollVariableId,
            AnswerText = answerText,
            RiskLevel = riskLevel,
        };

        return answer;
    }

    [Fact]
    public async Task Handle_EmptyAnswerList_DoesNotQueryRepositoryAndAddsEmptyBatch()
    {
        var request = new CreateAnswerListCommand
        {
            Answers = []
        };

        CreateCommandResponse<List<Answer>> result = await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(
            r => r.GetByPollInstanceIdAsync(It.IsAny<int>()), Times.Never);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task Handle_NewAnswers_AddsToBatchAndDoesNotUpdate()
    {
        var incoming = new List<Answer>
        {
            BuildAnswer(1),
            BuildAnswer(1)
        };

        var request = new CreateAnswerListCommand
        {
            Answers = incoming.Select(a => a.ToDto()).ToList()
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer>());

        CreateCommandResponse<List<Answer>> result = await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(r => r.GetByPollInstanceIdAsync(1), Times.Once);
        _answerRepositoryMock.Verify(
            r => r.UpdateAnswerTextAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingAnswer_UpdatesInsteadOfAdding()
    {
        Answer persistedAnswer = BuildAnswer(1, 10, 5, "old answer");
        persistedAnswer.Id = 1;

        Answer incomingAnswer = BuildAnswer(1, 10, 5, "new answer");

        var request = new CreateAnswerListCommand
        {
            Answers = new List<AnswerDTO> { incomingAnswer.ToDto() }
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer> { persistedAnswer });

        CreateCommandResponse<List<Answer>> result = await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(
            r => r.UpdateAnswerTextAsync(persistedAnswer.Id, incomingAnswer.AnswerText, incomingAnswer.RiskLevel), Times.Once);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 0)), Times.Once);
    }

    [Fact]
    public async Task Handle_NewAndExistingAnswers_UpdatesExistingAndAddsNew()
    {
        Answer persistedAnswer = BuildAnswer(1, 10, 3, "old answer");
        persistedAnswer.Id = 1;

        Answer existingIncoming = BuildAnswer(1, 10, 4, "updated answer");
        Answer newIncoming = BuildAnswer(1, 20, 2, "new answer");

        var request = new CreateAnswerListCommand
        {
            Answers = new List<AnswerDTO>
            {
                existingIncoming.ToDto(),
                newIncoming.ToDto()
            }
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer> { persistedAnswer });

        await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(
            r => r.UpdateAnswerTextAsync(persistedAnswer.Id, existingIncoming.AnswerText, existingIncoming.RiskLevel), Times.Once);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 1)), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleAnswersSamePollInstance_OnlyQueriesRepositoryOnce()
    {
        var incoming = new List<Answer>
        {
            BuildAnswer(1, 10),
            BuildAnswer(1, 20),
            BuildAnswer(1, 30)
        };

        var request = new CreateAnswerListCommand
        {
            Answers = incoming.Select(a => a.ToDto()).ToList()
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer>());

        await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(r => r.GetByPollInstanceIdAsync(1), Times.Once);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 3)), Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleDistinctPollInstances_QueriesRepositoryOncePerPollInstance()
    {
        var incoming = new List<Answer>
        {
            BuildAnswer(1, 10),
            BuildAnswer(2, 20)
        };

        var request = new CreateAnswerListCommand
        {
            Answers = incoming.Select(a => a.ToDto()).ToList()
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer>());
        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(2))
            .ReturnsAsync(new List<Answer>());

        await _handler.Handle(request, CancellationToken.None);

        _answerRepositoryMock.Verify(r => r.GetByPollInstanceIdAsync(1), Times.Once);
        _answerRepositoryMock.Verify(r => r.GetByPollInstanceIdAsync(2), Times.Once);
        _answerRepositoryMock.Verify(
            r => r.AddBatchAsync(It.Is<List<Answer>>(list => list.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsDuringUpdate_LogsErrorAndRethrows()
    {
        Answer persistedAnswer = BuildAnswer(1, 10, 4, "old answer");
        persistedAnswer.Id = 1;

        Answer incomingAnswer = BuildAnswer(1, 10, 5, "updated answer");

        var request = new CreateAnswerListCommand
        {
            Answers = new List<AnswerDTO> { incomingAnswer.ToDto() }
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer> { persistedAnswer });

        var expectedException = new InvalidOperationException("DB Connection Error");

        _answerRepositoryMock
            .Setup(r => r.UpdateAnswerTextAsync(persistedAnswer.Id, incomingAnswer.AnswerText, incomingAnswer.RiskLevel))
            .ThrowsAsync(expectedException);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RepositoryThrowsDuringAddBatch_LogsErrorAndRethrows()
    {
        var incomingAnswer = BuildAnswer(1, 10, 3, "new answer");

        var request = new CreateAnswerListCommand
        {
            Answers = new List<AnswerDTO> {incomingAnswer.ToDto() }
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer>());

        var expectedException = new Exception("Batch insert failed.");
        _answerRepositoryMock
            .Setup(r => r.AddBatchAsync(It.IsAny<List<Answer>>()))
            .ThrowsAsync(expectedException);

        await Assert.ThrowsAsync<Exception>(async () => await _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResponse_ContainingAllProcessedAnswers()
    {
        var incoming = new List<Answer>
        {
            BuildAnswer(1, 10),
            BuildAnswer(1, 20)
        };

        var request = new CreateAnswerListCommand
        {
            Answers = incoming.Select(a => a.ToDto()).ToList()
        };

        _answerRepositoryMock
            .Setup(r => r.GetByPollInstanceIdAsync(1))
            .ReturnsAsync(new List<Answer>());

        CreateCommandResponse<List<Answer>> result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);
        Assert.Equal(1, result.SuccessfullImports);
    }
}