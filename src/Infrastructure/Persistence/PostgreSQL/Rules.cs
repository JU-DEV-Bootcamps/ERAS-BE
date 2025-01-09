using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class Rules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public int ReportId { get; set; }

        [ForeignKey("ReportId")]
        public virtual Report Report { get; set; } = default!;

        public int? ComponentVariableId { get; set; }

        [ForeignKey("ComponentVariableId")]
        public virtual ComponentVariable ComponentVariable { get; set; } = default!;
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}