using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Dtos;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.DTOs
{
    public class PollInstanceDTO
    {
        public string Uuid { get; set; } = string.Empty;
        public StudentDTO Student { get; set; } = default!;
        public ICollection<AnswerDTO> Answers { get; set; } = [];
        public AuditInfo Audit { get; set; } = default!;
        public DateTime FinishedAt { get; set; }
    }
}
