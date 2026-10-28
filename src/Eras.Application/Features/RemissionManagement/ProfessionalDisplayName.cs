using Eras.Application.DTOs.UsersManagement;

namespace Eras.Application.Features.RemissionManagement;

/// <summary>
/// Builds the name under which a professional is stored on an intervention. It must match the label
/// the frontend shows in its professional selectors (first and last name, or the email when both are blank).
/// </summary>
public static class ProfessionalDisplayName
{
    public static string From(ErasUserDTO User)
    {
        string fullName = $"{User.FirstName} {User.LastName}".Trim();
        return fullName.Length > 0 ? fullName : User.Email;
    }
}
