namespace Eras.Domain.Entities;

/// <summary>
/// Fixed list of types of a student intervention entry. The first four belong to
/// <see cref="StudentInterventionKind.RiskIdentified"/>, the last four to
/// <see cref="StudentInterventionKind.SupportReceived"/> (see <see cref="StudentInterventionTypes"/>).
/// </summary>
public enum StudentInterventionType
{
    Academic,
    Financial,
    Attendance,
    Personal,
    Tutoring,
    Advising,
    FinancialAid,
    Referral
}
