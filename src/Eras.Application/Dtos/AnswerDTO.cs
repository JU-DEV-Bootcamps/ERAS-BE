using Eras.Domain.Common;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.Dtos
{
    public class AnswerDTO
    {
        public string Answer { get; set; } = String.Empty;
        public double Score { get; set; }
        public int PollInstanceId { get; set; }
        public int PollVariableId { get; set; }

        public StudentDTO? Student { get; set; }
        public AuditInfo? Audit { get; set; } = default!;
    }
}