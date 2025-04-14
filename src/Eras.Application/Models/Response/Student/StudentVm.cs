using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Common;
using Eras.Domain.Entities;

namespace Eras.Application.Models.Response.Student
{
    public class StudentVm : BaseEntity, IAuditableEntity
    {
        public string Uuid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StudentDetail StudentDetail { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
        public int CohortId { get; set; }
        public Cohort? Cohort { get; set; } = default!;
        public bool IsComplete { get; set; }
    }
}
