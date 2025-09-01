using System.ComponentModel;

namespace Eras.Domain.Entities;
public static class JURemissionsConstants
{
    public enum RemissionsStatus
    {
        [Description("Remission it's created")]
        Created,
        [Description("Remission remitted to a professional")]
        Remitted,
        [Description("Remission assigned to a JUProfessional but not working on it")]
        OnHold,
        [Description("Remission it's in progress by a professional")]
        InProgress,
        [Description("Remission it's completed")]
        Completed
    }
}
