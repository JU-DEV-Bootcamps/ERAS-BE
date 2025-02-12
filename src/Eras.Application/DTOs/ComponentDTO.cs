using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs
{
    public class ComponentDTO
    {
        public string Name { get; set; } = String.Empty;
        public ICollection<VariableDTO> Variables { get; set; } = [];
    }
}
