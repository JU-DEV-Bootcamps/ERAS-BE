using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities;
public static class RemissionsConstants
{
    public enum RemissionsStatus
    {
        [Description("Remission it's created")]
        Created,
        [Description("Remission remitted to a professional")]
        Remitted,
        [Description("Remission assigned to a professional but not working on it")]
        OnHold,
        [Description("Remission it's in progress by a professional")]
        InProgress,
        [Description("Remission it's completed")]
        Completed
    }
}
