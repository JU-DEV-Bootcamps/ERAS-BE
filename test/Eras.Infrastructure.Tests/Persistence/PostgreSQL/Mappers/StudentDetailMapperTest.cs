using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;
using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class StudentDetailMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_StudentDetailEntity_To_StudentDetail()
        {
            var entity = new StudentDetailEntity
            {
                Id = 1,
                StudentId = 123,
                EnrolledCourses = 5,
                GradedCourses = 4,
                TimeDeliveryRate = 90,
                AvgScore = 85,
                CoursesUnderAvg = 2,
                PureScoreDiff = 10,
                StandardScoreDiff = 5,
                LastAccessDays = 3,
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.StudentId, result.StudentId);
            Assert.Equal(entity.EnrolledCourses, result.EnrolledCourses);
            Assert.Equal(entity.GradedCourses, result.GradedCourses);
            Assert.Equal(entity.TimeDeliveryRate, result.TimeDeliveryRate);
            Assert.Equal(entity.AvgScore, result.AvgScore);
            Assert.Equal(entity.CoursesUnderAvg, result.CoursesUnderAvg);
            Assert.Equal(entity.PureScoreDiff, result.PureScoreDiff);
            Assert.Equal(entity.StandardScoreDiff, result.StandardScoreDiff);
            Assert.Equal(entity.LastAccessDays, result.LastAccessDays);
        }

        [Fact]
        public void ToPersistence_Should_Convert_StudentDetail_To_StudentDetailEntity()
        {
            var model = new StudentDetail
            {
                Id = 1,
                StudentId = 123,
                EnrolledCourses = 5,
                GradedCourses = 4,
                TimeDeliveryRate = 90,
                AvgScore = 85,
                CoursesUnderAvg = 2,
                PureScoreDiff = 10,
                StandardScoreDiff = 5,
                LastAccessDays = 3,
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.StudentId, result.StudentId);
            Assert.Equal(model.EnrolledCourses, result.EnrolledCourses);
            Assert.Equal(model.GradedCourses, result.GradedCourses);
            Assert.Equal(model.TimeDeliveryRate, result.TimeDeliveryRate);
            Assert.Equal(model.AvgScore, result.AvgScore);
            Assert.Equal(model.CoursesUnderAvg, result.CoursesUnderAvg);
            Assert.Equal(model.PureScoreDiff, result.PureScoreDiff);
            Assert.Equal(model.StandardScoreDiff, result.StandardScoreDiff);
            Assert.Equal(model.LastAccessDays, result.LastAccessDays);
        }
    }
}
