using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class StudentCourse
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(2)]
        public string Grade { get; set; } = string.Empty;

        public decimal Score { get; set; }

        [StringLength(255)]
        public string Notes { get; set; } = string.Empty;

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Courses Course { get; set; } = default!;

        public int StudentDetailsId { get; set; }

        [ForeignKey("StudentDetailsId")]
        public virtual StudentDetails Detail { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}