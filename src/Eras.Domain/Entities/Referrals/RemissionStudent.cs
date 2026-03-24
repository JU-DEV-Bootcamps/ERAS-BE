namespace Eras.Domain.Entities.Referrals;

public sealed class RemissionStudent : BaseEntity<StudentId>
{
    private RemissionStudent()
    {
    }

    public required StudentId StudentId { get; init; }

    internal static RemissionStudent Create(StudentId studentId)
    {
        return new RemissionStudent
        {
            Id = studentId,
            StudentId = studentId
        };
    }
}
