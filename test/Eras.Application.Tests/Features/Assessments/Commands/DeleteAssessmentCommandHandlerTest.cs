using Eras.Application.Contracts.Persistence.AssessmentManagement;
using Eras.Application.Features.RemissionManagement;
using Eras.Application.Features.RemissionManagement.Handlers;

using Moq;

namespace Eras.Application.Tests.Features.Assessments.Commands;

public class DeleteAssessmentCommandHandlerTest
{
    private readonly Mock<IAssessmentRepository> _mockRepository;
    private readonly DeleteAssessmentCommandHandler _handler;

    public DeleteAssessmentCommandHandlerTest()
    {
        _mockRepository = new Mock<IAssessmentRepository>();
        _handler = new DeleteAssessmentCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_DeleteAssessment_ShouldDeleteSuccessfully()
    {
        // Arrange
        var assessmentId = 1;
        var command = new DeleteAssessmentCommand(assessmentId);

        _mockRepository
            .Setup(x => x.DeleteAssessmentAsync(assessmentId));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockRepository.Verify(
            x => x.DeleteAssessmentAsync(assessmentId), Times.Once);
    }
}
