using Eras.Application.Validation;
using Eras.Domain.Entities;

using FluentAssertions;

namespace Eras.Application.Tests.Validation;

public class EvaluationDateRangeValidatorTests
{
    [Fact]
    public void EnsureWithinRange_ShouldNotThrow_WhenRequestedRangeIsInsideEvaluationRange()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31)
        };

        // Act
        Action act = () => EvaluationDateRangeValidator.EnsureWithinRange(
            evaluation,
            new DateTime(2025, 3, 1),
            new DateTime(2025, 5, 31));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureWithinRange_ShouldNotThrow_WhenRangesOverlap()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31)
        };

        // Act
        Action act = () => EvaluationDateRangeValidator.EnsureWithinRange(
            evaluation,
            new DateTime(2024, 12, 15),
            new DateTime(2025, 2, 1));

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureWithinRange_ShouldThrow_WhenRequestedRangeIsBeforeEvaluation()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31)
        };

        // Act
        Action act = () => EvaluationDateRangeValidator.EnsureWithinRange(
            evaluation,
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31));

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("There was an error during the import: The selected date range (2024-01-01 - 2024-12-31) is outside the evaluation's date range (2025-01-01 - 2025-12-31).");
    }

    [Fact]
    public void EnsureWithinRange_ShouldThrow_WhenRequestedRangeIsAfterEvaluation()
    {
        // Arrange
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31)
        };

        // Act
        Action act = () => EvaluationDateRangeValidator.EnsureWithinRange(
            evaluation,
            new DateTime(2026, 1, 1),
            new DateTime(2026, 12, 31));

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("There was an error during the import: The selected date range (2026-01-01 - 2026-12-31) is outside the evaluation's date range (2025-01-01 - 2025-12-31).");
    }

    [Theory]
    [InlineData("2025-01-01", "2025-12-31")]
    [InlineData("2025-12-31", "2026-01-31")]
    [InlineData("2024-12-01", "2025-01-01")]
    public void EnsureWithinRange_ShouldNotThrow_WhenRangesTouchOrOverlap(string requestedStart, string requestedEnd)
    {
        // Arrange
        var evaluation = new Evaluation
        {
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 12, 31)
        };

        // Act
        Action act = () => EvaluationDateRangeValidator.EnsureWithinRange(
            evaluation,
            DateTime.Parse(requestedStart),
            DateTime.Parse(requestedEnd));

        // Assert
        act.Should().NotThrow();
    }
}