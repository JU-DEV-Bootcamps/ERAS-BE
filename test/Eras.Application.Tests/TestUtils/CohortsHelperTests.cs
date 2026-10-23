using Eras.Application.Utils;

using Xunit;

namespace Eras.Application.Tests.TestUtils;

public class CohortsHelperTests
{
    [Theory]
    [InlineData("Cohort 1 (January 2024)", 2024, 1)]
    [InlineData("Cohort 2 (July 2026)", 2026, 7)]
    [InlineData("Cohort 1 (June 2024)", 2024, 6)]
    [InlineData("Cohort 2 (December 2025)", 2025, 12)]
    public void ParseCohortDate_ShouldParseEnglishMonth(string cohort, int expectedYear, int expectedMonth)
    {
        var result = CohortsHelper.ParseCohortDate(cohort);

        Assert.NotNull(result);
        Assert.Equal(new DateTime(expectedYear, expectedMonth, 1), result.Value);
    }

    [Theory]
    [InlineData("Cohort 1 (enero 2024)", 2024, 1)]
    [InlineData("Cohort 1 (junio 2026)", 2026, 6)]
    [InlineData("Cohort 2 (julio 2025)", 2025, 7)]
    [InlineData("Cohort 2 (diciembre 2024)", 2024, 12)]
    public void ParseCohortDate_ShouldParseSpanishMonth(string cohort, int expectedYear, int expectedMonth)
    {
        var result = CohortsHelper.ParseCohortDate(cohort);

        Assert.NotNull(result);
        Assert.Equal(new DateTime(expectedYear, expectedMonth, 1), result.Value);
    }

    [Theory]
    [InlineData("Cohort 1 (JANUARY 2024)", 2024, 1)]
    [InlineData("Cohort 1 (January 2024)", 2024, 1)]
    [InlineData("Cohort 1 (january 2024)", 2024, 1)]
    [InlineData("Cohort 2 (JULY 2024)", 2024, 7)]
    public void ParseCohortDate_ShouldBeCaseInsensitive(string cohort, int expectedYear, int expectedMonth)
    {
        var result = CohortsHelper.ParseCohortDate(cohort);

        Assert.NotNull(result);
        Assert.Equal(new DateTime(expectedYear, expectedMonth, 1), result.Value);
    }

    [Theory]
    [InlineData("Cohort 1 ( January 2024 )")]
    [InlineData("Cohort 1 (  January   2024  )")]
    [InlineData("Something (June 2024)")]
    [InlineData("(July 2024)")]
    public void ParseCohortDate_ShouldParseMonthAndYearWithWhitespace(string cohort)
    {
        var result = CohortsHelper.ParseCohortDate(cohort);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Cohort 1")]
    [InlineData("Cohort 1 ()")]
    [InlineData("Cohort 1 (2024)")]
    [InlineData("Cohort 1 (January)")]
    [InlineData("Cohort 1 (NotAMonth 2024)")]
    [InlineData("Cohort 1 (January ABCD)")]
    [InlineData("Cohort 1 January 2024")]
    public void ParseCohortDate_ShouldReturnNull_ForInvalidCohort(string cohort)
    {
        var result = CohortsHelper.ParseCohortDate(cohort);

        Assert.Null(result);
    }

    [Theory]
    [InlineData(2024, 1, "Cohort 1 (2024)")]
    [InlineData(2024, 3, "Cohort 1 (2024)")]
    [InlineData(2024, 6, "Cohort 1 (2024)")]
    [InlineData(2024, 7, "Cohort 2 (2024)")]
    [InlineData(2024, 9, "Cohort 2 (2024)")]
    [InlineData(2024, 12, "Cohort 2 (2024)")]
    public void GetCohort_ShouldReturnExpectedCohort(int year, int month, string expected)
    {
        var date = new DateTime(year, month, 15);

        var result = CohortsHelper.GetCohort(date);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "Cohort 1 (2024)")]
    [InlineData(6, "Cohort 1 (2024)")]
    [InlineData(7, "Cohort 2 (2024)")]
    [InlineData(12, "Cohort 2 (2024)")]
    public void GetCohort_ShouldHandleCohortBoundaries(int month, string expected)
    {
        var date = new DateTime(2024, month, 1);

        var result = CohortsHelper.GetCohort(date);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldReturnSingleCohort_WhenDatesAreWithinSameCohort()
    {
        var startDate = new DateTime(2024, 2, 1);
        var endDate = new DateTime(2024, 5, 31);

        var result = CohortsHelper.GetCohortsFromDateRange(startDate, endDate);

        Assert.Single(result);
        Assert.Equal("Cohort 1 (2024)", result[0]);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldReturnTwoCohorts_WhenRangeCrossesSixMonthBoundary()
    {
        var startDate = new DateTime(2024, 5, 1);
        var endDate = new DateTime(2024, 8, 1);

        var result = CohortsHelper.GetCohortsFromDateRange(startDate, endDate);

        Assert.Equal(new[]{"Cohort 1 (2024)", "Cohort 2 (2024)"}, result);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldReturnMultipleYears()
    {
        var startDate = new DateTime(2023, 10, 1);
        var endDate = new DateTime(2024, 8, 1);

        var result = CohortsHelper.GetCohortsFromDateRange(startDate, endDate);

        Assert.Equal(new[] { "Cohort 2 (2023)", "Cohort 1 (2024)", "Cohort 2 (2024)" }, result);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldHandleReversedDates()
    {
        var startDate = new DateTime(2024, 8, 1);
        var endDate = new DateTime(2024, 2, 1);

        var result = CohortsHelper.GetCohortsFromDateRange(startDate, endDate);

        Assert.Equal(new[] { "Cohort 1 (2024)", "Cohort 2 (2024)" }, result);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldReturnSingleCohort_WhenStartAndEndAreSame()
    {
        var date = new DateTime(2024, 4, 15);

        var result = CohortsHelper.GetCohortsFromDateRange(date, date);

        Assert.Single(result);
        Assert.Equal("Cohort 1 (2024)", result[0]);
    }

    [Fact]
    public void GetCohortsFromDateRange_ShouldIncludeBothCohorts_WhenRangeStartsAndEndsOnBoundary()
    {
        var startDate = new DateTime(2024, 6, 30);
        var endDate = new DateTime(2024, 7, 1);

        var result = CohortsHelper.GetCohortsFromDateRange(startDate, endDate);

        Assert.Equal(new[] { "Cohort 1 (2024)", "Cohort 2 (2024)" }, result);
    }

    [Theory]
    [InlineData("Cohort 1 (January 2024)", 2024, 1, 2024, 6, true)]
    [InlineData("Cohort 1 (June 2024)", 2024, 1, 2024, 6, true)]
    [InlineData("Cohort 2 (July 2024)", 2024, 7, 2024, 12, true)]
    [InlineData("Cohort 2 (December 2024)", 2024, 7, 2024, 12, true)]
    public void CohortInDateRange_ShouldReturnTrue_WhenCohortDateIsInRange(
        string cohort, int startYear, int startMonth, int endYear, int endMonth, bool expected)
    {
        var startDate = new DateTime(startYear, startMonth, 1);
        var endDate = new DateTime(endYear, endMonth, 28);

        var result = CohortsHelper.CohortInDateRange(cohort, startDate, endDate);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CohortInDateRange_ShouldReturnFalse_WhenCohortIsBeforeRange()
    {
        var cohort = "Cohort 1 (2023)";
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 6, 30);

        var result = CohortsHelper.CohortInDateRange(cohort, startDate, endDate);

        Assert.False(result);
    }

    [Fact]
    public void CohortInDateRange_ShouldReturnFalse_WhenCohortIsAfterRange()
    {
        var cohort = "Cohort 1 (2025)";
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var result = CohortsHelper.CohortInDateRange(cohort, startDate, endDate);

        Assert.False(result);
    }

    [Theory]
    [InlineData("Invalid Cohort")]
    [InlineData("")]
    [InlineData("Cohort 1")]
    [InlineData("Cohort 1 (invalid 2024)")]
    public void CohortInDateRange_ShouldReturnFalse_WhenCohortCannotBeParsed(
        string cohort)
    {
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 12, 31);

        var result = CohortsHelper.CohortInDateRange(cohort, startDate, endDate);

        Assert.False(result);
    }

    [Fact]
    public void GetPreviousCohortRange_ShouldReturnPreviousCohort()
    {
        var result = CohortsHelper.GetPreviousCohortRange();

        Assert.Equal(DateTimeKind.Utc, result.Start.Kind);
        Assert.Equal(DateTimeKind.Utc, result.End.Kind);
        Assert.True(result.Start < result.End);
    } 
}
