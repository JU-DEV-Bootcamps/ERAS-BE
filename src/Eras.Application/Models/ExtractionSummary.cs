namespace Eras.Application.Models;

/// <summary>
/// Outcome of extracting the respondents of a Cosmic Latte evaluation set: how many responses the
/// API returned, how many were extracted, and why the rest were left out.
/// </summary>
public sealed record ExtractionSummary(
    int Returned,
    int Extracted,
    int SkippedWithoutScore,
    int SkippedRequestFailed,
    int SkippedOutsideDateRange,
    int SkippedInvalidAnswers)
{
    public static ExtractionSummary Empty { get; } = new(0, 0, 0, 0, 0, 0);

    public int Skipped =>
        SkippedWithoutScore + SkippedRequestFailed + SkippedOutsideDateRange + SkippedInvalidAnswers;

    public override string ToString() =>
        $"Cosmic Latte returned {Returned} responses: {Extracted} extracted, {Skipped} skipped "
        + $"(without score: {SkippedWithoutScore}, request failed: {SkippedRequestFailed}, "
        + $"outside date range: {SkippedOutsideDateRange}, invalid answers: {SkippedInvalidAnswers}).";
}
