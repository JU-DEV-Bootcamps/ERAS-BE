namespace Eras.Application.DTOs.UsersManagement;

/// <summary>
/// The authenticated user's own profile: the stored user data plus read-only counters of the
/// work currently open for them. Same shape for every role.
/// </summary>
public class MyProfileDTO : ErasUserDTO
{
    /// <summary>Assessments created by or assigned to the user that are not finalized yet.</summary>
    public int ActiveAssessmentsCount { get; set; }

    /// <summary>Interventions created by the user that are not finalized yet.</summary>
    public int ActiveInterventionsCount { get; set; }

    public static MyProfileDTO From(ErasUserDTO User, int ActiveAssessments, int ActiveInterventions) => new()
    {
        Id = User.Id,
        Sub = User.Sub,
        Email = User.Email,
        FirstName = User.FirstName,
        LastName = User.LastName,
        Role = User.Role,
        IsSynced = User.IsSynced,
        EmployeeId = User.EmployeeId,
        Department = User.Department,
        Phone = User.Phone,
        Position = User.Position,
        About = User.About,
        Audit = User.Audit,
        ActiveAssessmentsCount = ActiveAssessments,
        ActiveInterventionsCount = ActiveInterventions,
    };
}
