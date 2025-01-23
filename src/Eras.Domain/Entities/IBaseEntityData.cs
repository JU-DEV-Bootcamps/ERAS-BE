using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public interface IBaseEntityData
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        /*         
        public string CreationUser { get; set; }
        public string ModificationUser { get; set; }
         */
    }
}
