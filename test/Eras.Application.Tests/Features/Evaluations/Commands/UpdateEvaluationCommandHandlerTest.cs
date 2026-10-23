using Eras.Application.Contracts.Persistence;
using Eras.Application.DTOs;
using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Commands;
using Eras.Application.Features.Evaluations.Commands.UpdateEvaluation;
using Eras.Domain.Entities;
using Eras.Error.Bussiness;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Eras.Application.Tests.Features.Evaluations.Commands;

public class UpdateEvaluationCommandHandlerTest
{
    private readonly Mock<IEvaluationRepository> _evaluationRepository;
    private readonly Mock<IPollRepository> _pollRepository;
    private readonly Mock<ILogger<UpdateEvaluationCommandHandler>> _logger;
    private readonly Mock<IMediator> _mediator;
    private readonly UpdateEvaluationCommandHandler _handler;

    public UpdateEvaluationCommandHandlerTest()
    {
        _evaluationRepository = new Mock<IEvaluationRepository>();
        _pollRepository = new Mock<IPollRepository>();
        _logger = new Mock<ILogger<UpdateEvaluationCommandHandler>>();
        _mediator = new Mock<IMediator>();
        _handler = new UpdateEvaluationCommandHandler(
            _evaluationRepository.Object,
            _pollRepository.Object,
            _logger.Object,
            _mediator.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenEvaluationDoesNotExist()
    {
        var request = new UpdateEvaluationCommand
        {
            EvaluationDTO = new EvaluationDTO
            {
                Id = 1,
                Name = "Updated Evaluation",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            }
        };

        _evaluationRepository
            .Setup(x => x.GetByIdForUpdateAsync(1))
            .ReturnsAsync((Evaluation?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(request, CancellationToken.None));

        _evaluationRepository.Verify(x => x.GetByIdForUpdateAsync(1), Times.Once);
        _evaluationRepository.Verify(x => x.UpdateAsync(It.IsAny<Evaluation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowExistingEvaluationNameException_WhenNameAlreadyExists()
    {
        var evaluation = new Evaluation
        {
            Id = 1,
            Name = "Old Name"
        };

        var request = new UpdateEvaluationCommand
        {
            EvaluationDTO = new EvaluationDTO
            {
                Id = 1,
                Name = "Existing Name",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            }
        };

        _evaluationRepository
            .Setup(x => x.GetByIdForUpdateAsync(1))
            .ReturnsAsync(evaluation);

        _evaluationRepository
            .Setup(x => x.GetByNameForUpdateAsync(
                1,
                "Existing Name"))
            .ReturnsAsync(new Evaluation
            {
                Id = 1,
                Name = "Existing Name"
            });

        await Assert.ThrowsAsync<ExistingEvaluationNameException>(
            () => _handler.Handle(request, CancellationToken.None));

        _evaluationRepository.Verify(x => x.UpdateAsync(It.IsAny<Evaluation>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEvaluation_WhenEvaluationExistsAndNoPollIsProvided()
    { 
        var evaluation = new Evaluation
        {
            Id = 1,
            Name = "Old Name",
            PollName = "null",
            Status = "Incomplete",
            Audit = new Domain.Common.AuditInfo()
            {
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = "new",
            }
        };

        var request = new UpdateEvaluationCommand
        {
            EvaluationDTO = new EvaluationDTO
            {
                Id = 1,
                Name = "Updated Name",
                PollName = "null",
                Country = "US",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            }
        };

        _evaluationRepository
            .Setup(x => x.GetByIdForUpdateAsync(1))
            .ReturnsAsync(evaluation);

        _evaluationRepository
            .Setup(x => x.GetByNameForUpdateAsync(1, "Updated Name"))
            .ReturnsAsync((Evaluation?)null);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Success", result.Message);

        Assert.Equal("Updated Name", evaluation.Name);
        Assert.Equal("US", evaluation.Country);

        _evaluationRepository.Verify(x => x.UpdateAsync(evaluation), Times.Once);

        _mediator.Verify(
            x => x.Publish(It.Is<EvaluationCreatedEvent>(e => e.EvaluationId == 1),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCompleteEvaluationAndCreatePoll_WhenValidPollIsProvided()
    {
        var evaluation = new Evaluation
        {
            Id = 1,
            Name = "Old Name",
            PollName = "new/poll",
            Status = "Incomplete",
            Audit = new Domain.Common.AuditInfo()
            {
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = "new",
            }
        };

        var poll = new Poll
        {
            Id = 2,
            Name = "Customer Poll"
        };

        var request = new UpdateEvaluationCommand
        {
            EvaluationDTO = new EvaluationDTO
            {
                Id = 1,
                Name = "Updated Evaluation",
                PollName = "Customer Poll",
                Country = "US",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            }
        };

        _evaluationRepository
            .Setup(x => x.GetByIdForUpdateAsync(1))
            .ReturnsAsync(evaluation);

        _evaluationRepository
            .Setup(x => x.GetByNameForUpdateAsync(1, "Updated Evaluation"))
            .ReturnsAsync((Evaluation?)null);

        _pollRepository
            .Setup(x => x.GetByNameAsync("Customer Poll"))
            .ReturnsAsync(poll);

        var result = await _handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Success);

        Assert.Equal("Customer Poll", evaluation.PollName);
        Assert.Equal("Complete", evaluation.Status);

        Assert.Equal(2, request.EvaluationDTO.PollId);
        Assert.Equal(1, request.EvaluationDTO.Id);

        _mediator.Verify(
            x => x.Send(
                It.Is<CreateEvaluationPollCommand>(c => c.EvaluationDTO.PollId == 2 && c.EvaluationDTO.Id == 1),
                It.IsAny<CancellationToken>()), Times.Once);

        _evaluationRepository.Verify(x => x.UpdateAsync(evaluation), Times.Once);

        _mediator.Verify(
            x => x.Publish(
                It.Is<EvaluationCreatedEvent>(e => e.EvaluationId == 1),
                It.IsAny<CancellationToken>()),Times.Once);
    }
}
