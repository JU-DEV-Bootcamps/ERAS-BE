using Eras.Application.Dtos;
using Eras.Application.DTOs; 
using Eras.Domain.Entities; 
using System.Globalization;

namespace Eras.Application.Mappers;

public static class StudentMapper
{
    public static Student ToDomain(this StudentDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        Cohort cohort = dto.Cohort?.ToDomain();
        return new Student
        {
            Id = dto.Id,
            Uuid = dto.Uuid,
            Name = dto.Name,
            Email = dto.Email,
            Cohort = cohort,
            CohortId = cohort!=null ? cohort.Id : 0,
            StudentDetail = dto.StudentDetail?.ToDomain(),
            Audit = dto.Audit,
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
        };
    }
}
