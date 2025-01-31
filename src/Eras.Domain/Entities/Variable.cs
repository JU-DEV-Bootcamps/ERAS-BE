using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    [Table("variables")]
    public class Variable : BaseEntity, IAuditInfo
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        public int ComponentId { get; set; }
        public Component Component { get; set; } = default!;
        
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}