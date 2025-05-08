using Eras.Application.DTOs;
using Eras.Domain.Entities;
using Eras.Application.Mappers;
namespace Eras.Application.Tests.Mappers;
public class StudentDetailMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_StudentDetailDTO_To_StudentDetail()
    {
        var dto = new StudentDetailDTO()
        {
            StudentId = 1,
            EnrolledCourses = 1,
            GradedCourses = 1,
            TimeDeliveryRate = 1,
            AvgScore = 10,
            CoursesUnderAvg = 5,
            PureScoreDiff = 10,
            StandardScoreDiff = 5,
            LastAccessDays = 12,

        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.StudentId, result.StudentId);
        Assert.Equal(dto.EnrolledCourses, result.EnrolledCourses);
        Assert.Equal(dto.GradedCourses, result.GradedCourses);
        Assert.Equal(dto.TimeDeliveryRate, result.TimeDeliveryRate);
        Assert.Equal(dto.AvgScore, result.AvgScore);
        Assert.Equal(dto.CoursesUnderAvg, result.CoursesUnderAvg);
        Assert.Equal(dto.PureScoreDiff, result.PureScoreDiff);
        Assert.Equal(dto.LastAccessDays, result.LastAccessDays);
        Assert.Equal(dto.StandardScoreDiff, result.StandardScoreDiff);
    }

    [Fact]
    public void ToDto_Should_Convert_StudentDetail_To_StudentDetailDto()
    {
        var model = new StudentDetail()
        {
            StudentId = 1,
            EnrolledCourses = 1,
            GradedCourses = 1,
            TimeDeliveryRate = 1,
            AvgScore = 10,
            CoursesUnderAvg = 5,
            PureScoreDiff = 10,
            StandardScoreDiff = 5,
            LastAccessDays = 12,

        };
        var result = model.ToDto();
        Assert.NotNull(result);
        Assert.Equal(model.StudentId, result.StudentId);
        Assert.Equal(model.EnrolledCourses, result.EnrolledCourses);
        Assert.Equal(model.GradedCourses, result.GradedCourses);
        Assert.Equal(model.TimeDeliveryRate, result.TimeDeliveryRate);
        Assert.Equal(model.AvgScore, result.AvgScore);
        Assert.Equal(model.CoursesUnderAvg, result.CoursesUnderAvg);
        Assert.Equal(model.PureScoreDiff, result.PureScoreDiff);
        Assert.Equal(model.LastAccessDays, result.LastAccessDays);
        Assert.Equal(model.StandardScoreDiff, result.StandardScoreDiff);
    }
}
