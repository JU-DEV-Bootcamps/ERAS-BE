using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Dtos;
using Eras.Domain.Common;

namespace Eras.Application.DTOs
{
    public class VariableDTO
    {
        public string Name { get; set; } = string.Empty;

        public int Position { get; set; } = 0;
        public string Type { get; set; } = "";

        public AnswerDTO? Answer { get; set; }
        public AuditInfo? Audit { get; set; } = default!;
    }
}
