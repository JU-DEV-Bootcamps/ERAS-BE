using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class StudentAnswer
    {
        public string Variable { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Component { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
