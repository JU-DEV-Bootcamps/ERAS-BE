using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Handlers;
using Eras.Application.Services;

using Moq;

namespace Eras.Application.Tests.Features.Evaluations.Handlers;

public class AnswerSubmittedEventHandlerTest
{
    [Fact]
    public async Task Handle_ShouldUpdateEvaluationStatus()
    {
        // Arrange
        var updater = new Mock<IEvaluationStatusUpdater>();
        var handler = new AnswerSubmittedEventHandler(updater.Object);

        var notification = new AnswerSubmittedEvent(1);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        updater.Verify(
            x => x.UpdateStatusAsync(1),
            Times.Once);
    }
}
