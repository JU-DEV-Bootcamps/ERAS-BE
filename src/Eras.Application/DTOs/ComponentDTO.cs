﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eras.Domain.Common;

namespace Eras.Application.DTOs
{
    [ExcludeFromCodeCoverage]
    public class ComponentDTO
    {
        public string Name { get; set; } = String.Empty;
        public ICollection<VariableDTO> Variables { get; set; } = [];
        public AuditInfo? Audit { get; set; } = default!;
    }
}
