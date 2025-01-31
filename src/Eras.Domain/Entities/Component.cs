using System.ComponentModel.DataAnnotations.Schema;
using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    [Table("components")]
    public class Component : BaseEntity, IAuditInfo
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        public ICollection<Variable> Variables { get; set; } = [];
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}