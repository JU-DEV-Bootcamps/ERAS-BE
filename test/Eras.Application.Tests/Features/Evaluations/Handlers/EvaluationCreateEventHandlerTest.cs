using Eras.Application.Events;
using Eras.Application.Features.Evaluations.Handlers;
using Eras.Application.Services;

using Moq;

namespace Eras.Application.Tests.Features.Evaluations.Handlers;

public class EvaluationCreateEventHandlerTest
{
    [Fact]
    public async Task Handler_ShouldCreateEvaluationForEvaluation()
    {
        // Arrange
        var updater = new Mock<IEvaluationStatusUpdater>();
        var handler = new EvaluationCreatedEventHandler(updater.Object);

        var notification = new EvaluationCreatedEvent(1);

        // Act
        await handler.Handle(notification, CancellationToken.None);

        // Assert
        updater.Verify(x => x.UpdateStatusAsync(1), Times.Once);
    }
}
