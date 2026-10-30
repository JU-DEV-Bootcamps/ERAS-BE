namespace Eras.Domain.Entities;

/// <summary>Which <see cref="StudentInterventionType"/> values are valid for each <see cref="StudentInterventionKind"/>.</summary>
public static class StudentInterventionTypes
{
    private static readonly IReadOnlyList<StudentInterventionType> _riskTypes =
    [
        StudentInterventionType.Academic,
        StudentInterventionType.Financial,
        StudentInterventionType.Attendance,
        StudentInterventionType.Personal
    ];

    private static readonly IReadOnlyList<StudentInterventionType> _supportTypes =
    [
        StudentInterventionType.Tutoring,
        StudentInterventionType.Advising,
        StudentInterventionType.FinancialAid,
        StudentInterventionType.Referral
    ];

    public static IReadOnlyList<StudentInterventionType> For(StudentInterventionKind Kind) => Kind switch
    {
        StudentInterventionKind.RiskIdentified => _riskTypes,
        StudentInterventionKind.SupportReceived => _supportTypes,
        _ => []
    };

    public static bool IsValidFor(StudentInterventionKind Kind, StudentInterventionType Type) =>
        For(Kind).Contains(Type);
}
