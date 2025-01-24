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
        public string Name { get; set; } = string.Empty;
        public int PollId { get; set; }
        public virtual Poll Poll { get; set; }
        public int Position { get; set; }
        public int? ParentId { get; set; }
        public virtual ComponentVariable Parent { get; set; } = default!;        
        
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
