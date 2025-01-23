using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Domain.Entities
{
    public class ComponentVariable : IBaseEntityData
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty; // textual question? ¿Hace cuanto se graduó como bachiller?

        public int? PollId { get; set; } // es necesario el id? no es suficiente con Poll poll para marcar la relacion?  
        public virtual Poll Poll { get; set; }


        public int Position { get; set; } // Position inside poll, like index starting from 1


        public int? ParentId { get; set; }  // Component that groups several variables, null for Components and for variables indicates wich components
        public virtual ComponentVariable Parent { get; set; } = default!;  // Component that groups several variables, null for Components and for variables indicates wich components
        
        
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ComponentVariable(int id, string name, int pollId, int position, int? parentId, DateTime createdDate, DateTime modifiedDate)
        {
            this.Id = id;
            this.Name = name;
            this.PollId = pollId;
            this.Position = position;
            this.ParentId = parentId;
            this.CreatedDate = createdDate;
            this.ModifiedDate = modifiedDate;            
        }
    }
}
