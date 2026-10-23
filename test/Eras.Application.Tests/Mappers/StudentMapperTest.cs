using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs;
using Eras.Application.Mappers;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Tests.Mappers;
public class StudentMapperTest
{
    [Fact]
    public void ToDomain_Should_Convert_StudentDTO_To_Student()
    {
        var dto = new StudentDTO()
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            IsImported = true,
        };
        var result = dto.ToDomain();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.IsImported, result.IsImported);
    }

    [Fact]
    public void ToDto_Should_Convert_PollVersion_To_PollVersionDto()
    {
        var dto = new Student()
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            IsImported = true,
        };
        var result = dto.ToDto();
        Assert.NotNull(result);
        Assert.Equal(dto.Uuid, result.Uuid);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.IsImported, result.IsImported);
    }

    [Fact]
    public void ExtractStudentDTO_ToStudentDTO()
    {
        var dto = new StudentImportDto()
        {
            SISId = "Uuid",
            Name = "Name",
            Email = "Email",
        };
        var result = dto.ExtractStudentDTO();

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(dto.SISId, result.Uuid);
        Assert.Equal(dto.Email, result.Email);
    }

    [Fact]
    public void ExtractStudentDetailDto_ToStudentDetailDTO()
    {
        var dto = new StudentImportDto()
        {
            SISId = "Uuid",
            Name = "Name",
            Email = "Email",
            EnrolledCourses = 1,
            GradedCourses = 1,
            TimelySubmissions = 1,
            AverageScore = 5,
            CoursesBelowAverage = 2,
            RawScoreDifference = 3,
            StandardScoreDifference = 4,
            DaysSinceLastAccess = 5,
        };
        var result = StudentMapper.ExtractStudentDetailDto(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.EnrolledCourses, result.EnrolledCourses);
        Assert.Equal(dto.GradedCourses, result.GradedCourses);
        Assert.Equal(dto.TimelySubmissions, result.TimeDeliveryRate);
        Assert.Equal(dto.AverageScore, result.AvgScore);
        Assert.Equal(dto.CoursesBelowAverage, result.CoursesUnderAvg);
        Assert.Equal(dto.RawScoreDifference, result.PureScoreDiff);
        Assert.Equal(dto.StandardScoreDifference, result.StandardScoreDiff);
        Assert.Equal(dto.DaysSinceLastAccess, result.LastAccessDays);
    }

    [Fact]
    public void ToDomain_Should_Map_StudentDetail_When_Provided()
    {
        var studentDetail = new StudentDetailDTO { };

        var dto = new StudentDTO
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            StudentDetail = studentDetail,
        };

        var result = dto.ToDomain();

        Assert.NotNull(result.StudentDetail);
    }

    [Fact]
    public void ToDomain_Should_Create_Automatic_Audit_When_Audit_Is_Null()
    {
        var dto = new StudentDTO
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            Cohort = new CohortDTO(),
            Audit = null,
        };

        var before = DateTime.UtcNow;

        var result = dto.ToDomain();

        var after = DateTime.UtcNow;

        Assert.NotNull(result.Audit);
        Assert.Equal("Automatic mapper", result.Audit.CreatedBy);
        Assert.InRange(result.Audit.CreatedAt, before, after);
    }

    [Fact]
    public void ToDomain_Should_Preserve_Audit_When_Provided()
    {
        var audit = new AuditInfo
        {
            CreatedBy = "Test User",
            CreatedAt = new DateTime(2025, 1, 1),
            ModifiedAt = new DateTime(2025, 2, 1),
        };

        var dto = new StudentDTO
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            Audit = audit,
        };

        var result = dto.ToDomain();

        Assert.Same(audit, result.Audit);
    }

    [Fact]
    public void ToDomain_Should_Set_Cohort_To_Null_And_CohortId_To_Zero_When_Cohort_Is_Null()
    {
        var dto = new StudentDTO
        {
            Uuid = "Uuid",
            Name = "Name",
            Email = "Email",
            Cohort = null,
        };

        var result = dto.ToDomain();

        Assert.Null(result.Cohort);
        Assert.Equal(0, result.CohortId);
    }
}
