using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public interface IMapper<in TSource, out TDestination>
{
    TDestination Map(TSource source);
}

public sealed class StudentProfileMapper : IMapper<StudentProfileDto, Student>
{
    public Student Map(StudentProfileDto source)
    {
        return new Student
        {
            Id = source.Id,
            Name = source.Name
        };
    }
}
