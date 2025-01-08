using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CosmicLatteStatus
    {
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public CosmicLatteStatus(bool Status)
        {
            this.Status = Status;
            this.DateTime = DateTime.Now;
        }
    }
}
