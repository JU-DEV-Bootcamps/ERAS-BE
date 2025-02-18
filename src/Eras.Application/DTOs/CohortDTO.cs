using Eras.Application.Dtos;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs
{
    public class CohortDTO
    {
        public string Name { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public AuditInfo Audit { get; set; } = default!;
    }
}
