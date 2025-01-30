namespace Eras.Domain.Entities
{
    public class StudentDetail
    {
        public int EnrolledCourses { get; set; }
        public int GradedCourses { get; set; }
        public int TimeDeliveryRate { get; set; }
        public decimal AvgScore { get; set; }
        public decimal CoursesUnderAvg { get; set; }
        public decimal PureScoreDiff { get; set; }
        public decimal StandardScoreDiff { get; set; }
        public int LastAccessDays { get; set; }
    }
}
