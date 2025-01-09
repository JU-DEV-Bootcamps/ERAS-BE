using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RiskLevel { get; set; }

        public int PollId { get; set; }

        [ForeignKey("PollId")]
        public virtual Polls Poll { get; set; }

        public int ComponentVariableId { get; set; }

        [ForeignKey("ComponentVariableId")]
        public virtual ComponentVariable ComponentVariable { get; set; }

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}