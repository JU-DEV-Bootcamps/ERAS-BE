using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers;

public static class StudentMapper
{
    public static StudentDetail CreateEmptyStudentDetail(StudentDTO Dto)
    {
        var audit = new AuditInfo()
        {
            CreatedBy = "Automatic mapper",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
        };
        return new StudentDetail
        {
            StudentId = Dto.Id,
            Audit = audit,
        };
    }
    public static Student ToDomain(this StudentDTO Dto)
    {
        ArgumentNullException.ThrowIfNull(Dto);
        Cohort? cohort = Dto.Cohort?.ToDomain();
        StudentDetail details = Dto.StudentDetail != null ? Dto.StudentDetail.ToDomain() : CreateEmptyStudentDetail(Dto);

        AuditInfo audit = Dto.Audit ?? new AuditInfo()
        {
            CreatedBy = "Automatic mapper",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
        };
        return new Student
        {
            Id = Dto.Id,
            Uuid = Dto.Uuid ?? "UUID not found",
            Name = Dto.Name,
            Email = Dto.Email,
            Cohort = cohort,
            CohortId = cohort != null ? cohort.Id : 0,
            StudentDetail = details,
            Audit = audit,
        };

    }
    public static StudentDTO ToDto(this Student Domain)
    {
        ArgumentNullException.ThrowIfNull(Domain);
        return new StudentDTO
        {
            Id = Domain.Id,
            Uuid = Domain.Uuid,
            Name = Domain.Name,
            Email = Domain.Email,
            Cohort = Domain.Cohort != null ? Domain.Cohort.ToDto() : new CohortDTO(),
            StudentDetail = Domain.StudentDetail?.ToDto(),
            Audit = Domain.Audit,
        };

    }

    public static StudentDTO ExtractStudentDTO(this StudentImportDto StudentImportDto)
    {
        ArgumentNullException.ThrowIfNull(StudentImportDto);
        return new StudentDTO()
        {
            Uuid = StudentImportDto.SISId,
            Name = StudentImportDto.Name,
            Email = StudentImportDto.Email,
            StudentDetail = ExctractStudentDetailDto(StudentImportDto),
            Cohort = new CohortDTO()
        };
    }

    public static StudentDetailDTO ExctractStudentDetailDto(StudentImportDto Dto)
    {
        return new StudentDetailDTO()
        {
            EnrolledCourses = Dto.EnrolledCourses,
            GradedCourses = Dto.GradedCourses,
            TimeDeliveryRate = Dto.TimelySubmissions,
            AvgScore = Dto.AverageScore,
            CoursesUnderAvg = Dto.CoursesBelowAverage,
            PureScoreDiff = Dto.RawScoreDifference,
            StandardScoreDiff = Dto.StandardScoreDifference,
            LastAccessDays = Dto.DaysSinceLastAccess,
            Audit = new AuditInfo()
            {
                CreatedBy = "CSV import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            }
        };
    }
}
