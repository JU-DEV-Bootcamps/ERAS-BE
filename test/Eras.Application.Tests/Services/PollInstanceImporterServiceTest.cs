using Eras.Application.Contracts.Persistence;
using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Application.Features.Answers.Commands.CreateAnswerList;
using Eras.Application.Features.PollInstances.Commands.CreatePollInstance;
using Eras.Application.Features.PollInstances.Queries.GetByUuidAndStudentId;
using Eras.Application.Mappers;
using Eras.Application.Models.Enums;
using Eras.Application.Models.Response.Common;
using Eras.Application.Services;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;
namespace Eras.Application.Tests.Services;


public class PollInstanceImporterTest
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IPollInstanceRepository> _repository;
    private readonly Mock<ILogger<PollInstanceImporter>> _logger;
    private readonly PollInstanceImporter _pollInstanceImporter;
    
    public PollInstanceImporterTest()
    {
        _mediator = new Mock<IMediator>();
        _repository = new Mock<IPollInstanceRepository>();
        _logger = new Mock<ILogger<PollInstanceImporter>>();
        _pollInstanceImporter = new PollInstanceImporter(_mediator.Object, _repository.Object, _logger.Object);

    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenAlreadyExists_ReturnsExistingPollInstance()
    {
        var student = new Student { Id = 1 };
        var existingPollInstance = new PollInstance { Id = 10 };

        _repository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(1, "poll-uuid", 5))
            .ReturnsAsync(true);

        _mediator
            .Setup(x => x.Send(
                It.Is<GetPollInstanceByUuidAndStudentIdQuery>(q =>
                    q.PollUuid == "poll-uuid" && q.StudentId == 1 &&
                    q.EvaluationId == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetQueryResponse<PollInstance>(existingPollInstance));

        var result = await _pollInstanceImporter.CreatePollInstanceAsync(
            student, "poll-uuid", DateTime.UtcNow, 5, new ImportContext(DateTime.UtcNow));

        Assert.True(result.Success);
        Assert.Equal("Already exists for this evaluation", result.Message);
        Assert.Same(existingPollInstance, result.Entity);
    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenDoesNotExist_CreatesPollInstance()
    {
        var finishedAt = new DateTime(2026, 1, 1);
        var context = new ImportContext(new DateTime(2025, 12, 1));
        var student = new Student { Id = 1 };

        _repository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(1, "poll-uuid", 5))
            .ReturnsAsync(false);

        var expectedResponse = new CreateCommandResponse<PollInstance>(
            new PollInstance { Id = 20 }, 1, "Created", true);
        var pollInstance = new PollInstance
        {
            Uuid = "poll-uuid",
            Student = student,
            FinishedAt = finishedAt,
            EvaluationId = 5,
            Audit = new Domain.Common.AuditInfo()
        };
        var query = new CreatePollInstanceCommand
        {
            PollInstance = pollInstance.ToDTO(),
        };

        _mediator
    .Setup(x => x.Send(
            It.Is<CreatePollInstanceCommand>(c =>
                c.PollInstance!.Uuid == "poll-uuid" &&
                c.PollInstance.EvaluationId == 5 &&
                c.PollInstance.FinishedAt == finishedAt &&
                c.PollInstance.LastVersion == context.VersionNumber &&
                c.PollInstance.LastVersionDate == context.InitDate &&
                c.PollInstance.AnswersHash == "hash"),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedResponse);

        var result = await _pollInstanceImporter.CreatePollInstanceAsync(
            student, "poll-uuid", finishedAt, 5, context, "hash");

        Assert.Same(expectedResponse, result);
    }

    [Fact]
    public async Task CreatePollInstanceAsync_WhenExceptionOccurs_ReturnsError()
    {
        var student = new Student { Id = 1 };

        _repository
            .Setup(x => x.ExistsForStudentAndEvaluationAsync(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Repository error"));

        var result = await _pollInstanceImporter.CreatePollInstanceAsync(
            student, "poll-uuid", DateTime.UtcNow, 5, new ImportContext(DateTime.UtcNow));

        Assert.False(result.Success);
        Assert.Equal("Error", result.Message);
        Assert.Equal(CommandEnums.CommandResultStatus.Error, result.Status);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Repository error")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAnswersAsync_WhenVariablesMatch_CreatesAnswers()
    {
        var variable = new Variable
        {
            PollVariableId = 100,
            Name = "Question",
            Position = 1
        };

        var component = new Component
        {
            Name = "Component",
            Variables = [variable]
        };

        var answer = new AnswerDTO();

        var poll = new PollDTO
        {
            Components =
            [
                new ComponentDTO
                {
                    Name = "Component",
                    Variables =
                    [
                        new VariableDTO
                        {
                            Name = "Question",
                            Position = 1,
                            Answer = answer
                        }
                    ]
                }
            ]
        };

        var context = new ImportContext(DateTime.Now);

        var createdPollInstance = new CreateCommandResponse<PollInstance>(
            new PollInstance { Id = 50 }, 1, "Created", true);

        await _pollInstanceImporter.CreateAnswersAsync(poll, [component], createdPollInstance, context);

        //_mediator.Verify(
        //    x => x.Send(It.Is<CreateCommandResponse<List<Answer>>>
        //    (It.Is<CreateAnswerListCommand>(), CancellationToken)()),
        //    Times.Once);

        _mediator.Verify(
            x => x.Send(
                It.Is<CreateAnswerListCommand>(c => c.Answers.Count != 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAnswersAsync_WhenNoVariablesMatch_SendsEmptyList()
    {
        var component = new Component
        {
            Name = "Component",
            Variables = []
        };

        var poll = new PollDTO
        {
            Components =
            [
                new ComponentDTO
                {
                    Name = "MissingComponent",
                    Variables = []
                }
            ]
        };

        var createdPollInstance = new CreateCommandResponse<PollInstance>(
            new PollInstance { Id = 50 },
            1,
            "Created",
            true);

        await _pollInstanceImporter.CreateAnswersAsync(
            poll,
            [component],
            createdPollInstance,
            new ImportContext(DateTime.Now));

        _mediator.Verify(
            x => x.Send(
                It.Is<CreateAnswerListCommand>(c => c.Answers.Count == 0),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
