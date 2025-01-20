using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Eras.Infrastructure.Persistence.PostgreSQL
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RiskLevel { get; set; }

        public int PollId { get; set; }

        [ForeignKey("PollId")]
        public virtual Polls Poll { get; set; } = default!;

        public int ComponentVariableId { get; set; }

        [ForeignKey("ComponentVariableId")]
        public virtual ComponentVariable ComponentVariable { get; set; } = default!;

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}