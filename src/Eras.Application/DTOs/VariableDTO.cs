using Eras.Application.Dtos;
using Eras.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs
{
    public class VariableDTO
    {
        public string Name { get; set; } = string.Empty;

        public int Position { get; set; } = 0;
        public string Type { get; set; } = "";

        public AnswerDTO? Answer { get; set; }
        public AuditInfo? Audit { get; set; } = default!;
        public VersionInfo Version { get; set; } = default!;
    }
}
