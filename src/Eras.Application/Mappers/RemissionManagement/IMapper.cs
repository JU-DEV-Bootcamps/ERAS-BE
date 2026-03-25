
using Eras.Application.DTOs.RemissionManagement;
using Eras.Domain.Entities.RemissionsManagement;

namespace Eras.Application.Mappers.RemissionManagement;

public interface IMapper<in TSource, out TDestination>
{
    TDestination Map(TSource source);
}

public sealed class StudentProfileMapper : IMapper<StudentProfileDto, StudentProfile>
{
    public StudentProfile Map(StudentProfileDto source)
    {
        return new StudentProfile
        {
            Id = source.Id ?? Guid.NewGuid(),
            StudentCode = source.StudentCode,
            FirstName = source.FirstName,
            LastName = source.LastName,
            SupportAndReferralHistory = source.SupportAndReferralHistory,
            CharacterizationOrCurrentContext = source.CharacterizationOrCurrentContext
        };
    }
}
