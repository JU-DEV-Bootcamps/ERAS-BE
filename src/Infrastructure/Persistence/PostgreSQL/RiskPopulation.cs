using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERAS.Infrastructure.Persistence.PostgreSQL
{
    public class RiskPopulation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [StringLength(100)]
        public string StudentName { get; set; } = string.Empty;

        public int EnrolledCourses { get; set; }

        public int GradedCourses { get; set; }

        public int TimeDeliveryRate { get; set; }

        public decimal AvgScore { get; set; }

        public decimal CoursesUnderAvg { get; set; }

        public decimal PureScoreDiff { get; set; }

        public decimal StandardScoreDiff { get; set; }

        public int LastAccessDays { get; set; }

        public decimal Score { get; set; }

        [StringLength(2)]
        public string Grade { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}