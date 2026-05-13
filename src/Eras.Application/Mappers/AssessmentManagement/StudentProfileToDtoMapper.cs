using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class StudentProfileToDtoMapper : IMapper<Student, StudentProfileDto>
{
    public StudentProfileDto Map(Student source)
    {
        return new StudentProfileDto
        {
            Id = source.Id,
            Name = source.Name
        };
    }
}