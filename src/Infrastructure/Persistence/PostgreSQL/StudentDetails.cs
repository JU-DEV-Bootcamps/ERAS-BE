using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.PostgreSQL
{
    public class StudentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EnrolledCourses { get; set; }

        public int GradedCourses { get; set; }

        public int TimeDeliveryRate { get; set; }

        public decimal AvgScore { get; set; }

        public decimal CoursesUnderAvg { get; set; }

        public decimal PureScoreDiff { get; set; }

        public decimal StandardScoreDiff { get; set; }

        public int LastAccessDays { get; set; }

        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Students Student { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

    }
}