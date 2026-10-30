using Eras.Application.DTOs.StudentInterventions;
using Eras.Application.Mappers.AssessmentManagement;
using Eras.Domain.Entities;

namespace Eras.Application.Mappers.StudentInterventions;

public sealed class StudentInterventionToDtoMapper : IMapper<StudentInterventionEntry, StudentInterventionDto>
{
    public StudentInterventionDto Map(StudentInterventionEntry source)
    {
        return new StudentInterventionDto
        {
            Id = source.Id,
            StudentId = source.StudentId,
            Kind = source.Kind,
            Type = source.Type,
            Description = source.Description,
            EntryDate = source.EntryDate,
            CreatedBy = source.Audit.CreatedBy,
            CreatedByName = source.CreatedByName,
            CreatedAt = source.Audit.CreatedAt
        };
    }
}
