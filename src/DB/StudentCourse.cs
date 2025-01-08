using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class StudentCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(2)]
        public string Grade { get; set; }

        public decimal Score { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Courses Course { get; set; }

        public int StudentDetailsId { get; set; }

        [ForeignKey("StudentDetailsId")]
        public virtual StudentDetails Detail { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}