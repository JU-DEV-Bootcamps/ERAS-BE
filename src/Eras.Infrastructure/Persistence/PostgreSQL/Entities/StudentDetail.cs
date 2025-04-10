using Eras.Domain.Common;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Entities
{
    public class StudentDetailEntity : BaseEntity, IAuditableEntity
    {
        public int EnrolledCourses { get; set; }
        public int GradedCourses { get; set; }
        public int TimeDeliveryRate { get; set; }
        public decimal AvgScore { get; set; }
        public decimal CoursesUnderAvg { get; set; }
        public decimal PureScoreDiff { get; set; }
        public decimal StandardScoreDiff { get; set; }
        public int LastAccessDays { get; set; }
        public int StudentId { get; set; }
        public StudentEntity Student { get; set; } = default!;
        public AuditInfo Audit { get; set; } = default!;
    }
}
