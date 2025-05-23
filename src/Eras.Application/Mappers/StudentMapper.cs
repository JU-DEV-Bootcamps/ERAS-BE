﻿using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using System.Globalization;

namespace Eras.Application.Mappers;

public static class StudentMapper
{
    public static StudentDetail CreateEmptyStudentDetail(StudentDTO dto)
    {
            AuditInfo audit = new AuditInfo()
            {
                CreatedBy = "Automatic mapper",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };
            return new StudentDetail
            {
                StudentId = dto.Id,
                Audit = audit,
            }; 
    }
    public static Student ToDomain(this StudentDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        Cohort cohort = dto.Cohort?.ToDomain();
        StudentDetail details = dto.StudentDetail != null ? dto.StudentDetail.ToDomain() : CreateEmptyStudentDetail(dto);

        AuditInfo audit = dto.Audit != null ? dto.Audit : new AuditInfo()
        {
            CreatedBy = "Automatic mapper",
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
        };
        return new Student
        {
            Id = dto.Id,
            Uuid = dto.Uuid,
            Name = dto.Name,
            Email = dto.Email,
            IsImported = dto.IsImported,
            Cohort = cohort,
            CohortId = cohort!=null ? cohort.Id : 0,
            StudentDetail = details,
            Audit = audit,
        };

    }
    public static StudentDTO ToDto(this Student domain)
    {
        ArgumentNullException.ThrowIfNull(domain);
        return new StudentDTO
        {
            Id = domain.Id,
            Uuid = domain.Uuid,
            Name = domain.Name,
            Email = domain.Email,
            Cohort = domain.Cohort?.ToDto(),
            IsImported = domain.IsImported,
            StudentDetail = domain.StudentDetail?.ToDto(),
            Audit = domain.Audit,
        };

    }

    public static StudentDTO ExtractStudentDTO(this StudentImportDto studentImportDto)
    {
        ArgumentNullException.ThrowIfNull(studentImportDto);
        return new StudentDTO()
        {
            Uuid = studentImportDto.SISId,
            Name = studentImportDto.Name,
            Email = studentImportDto.Email,
            StudentDetail = ExctractStudentDetailDto(studentImportDto),
            Cohort = new CohortDTO()
        };
    }

    public static StudentDetailDTO ExctractStudentDetailDto(StudentImportDto dto)
    {
        return new StudentDetailDTO()
        {
            EnrolledCourses = dto.EnrolledCourses,
            GradedCourses = dto.GradedCourses,
            TimeDeliveryRate = dto.TimelySubmissions,
            AvgScore = dto.AverageScore,
            CoursesUnderAvg = dto.CoursesBelowAverage,
            PureScoreDiff = dto.RawScoreDifference,
            StandardScoreDiff = dto.StandardScoreDifference,
            LastAccessDays = dto.DaysSinceLastAccess,
            Audit = new AuditInfo()
            {
                CreatedBy = "CSV import",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            }
        };        
    }
}
