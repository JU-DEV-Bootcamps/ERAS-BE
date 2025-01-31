using System.ComponentModel.DataAnnotations.Schema;
using Eras.Domain.Common;

namespace Eras.Domain.Entities
{
    [Table("poll")]
    public class Poll : BaseEntity, IAuditInfo
    {
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("version")]
        public string Version { get; set; } = string.Empty;
        [Column("uuid")]
        public string Uuid { get; set; } = string.Empty;
        [Column("created_by")]
        public string CreatedBy { get; set; } = string.Empty;
        [Column("modified_by")]
        public string ModifiedBy { get; set; } = string.Empty;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("modified_at")]
        public DateTime? ModifiedAt { get; set; }

    }
}
