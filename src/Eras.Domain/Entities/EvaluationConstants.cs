using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public static class EvaluationConstants
    {
        public enum EvaluationStatus
        {
            [Description("Evaluation is pending poll assignation")]
            Pending,
            [Description("Evaluation has at least a poll assigned and ready to start")]
            Ready,
            [Description("Evaluation has at least a pollInstance already answered")]
            InProgress,
            [Description("Evaluation has at least a pollInstance already answered and deadline is already past")]
            Completed,
            [Description("Evaluation has no pollInstance answered and deadline is already past")]
            Uncompleted
        }
    }
}
