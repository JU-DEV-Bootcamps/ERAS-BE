﻿using Eras.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eras.Application.DTOs
{
    public class StudentDetailDTO
    {
        public int StudentId { get; set; }
        public int EnrolledCourses { get; set; }
        public int GradedCourses { get; set; }
        public int TimeDeliveryRate { get; set; }
        public decimal AvgScore { get; set; }
        public decimal CoursesUnderAvg { get; set; }
        public decimal PureScoreDiff { get; set; }
        public decimal StandardScoreDiff { get; set; }
        public int LastAccessDays { get; set; }
        public AuditInfo? Audit { get; set; } = default!;
    }
}
