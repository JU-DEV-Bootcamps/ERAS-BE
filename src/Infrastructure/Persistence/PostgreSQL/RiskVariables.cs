using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class RiskVariables
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AnswerId { get; set; }

        public int RiskLevel { get; set; }

        [StringLength(50)]
        public string VariableName { get; set; }

        [StringLength(50)]
        public string ComponentName { get; set; }

        [StringLength(50)]
        public string PollName { get; set; }

        [StringLength(50)]
        public string StudentName { get; set; }

        public int StudentId { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}