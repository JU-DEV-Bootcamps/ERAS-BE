using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class CohortMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_CohortEntity_To_Cohort()
        {
            var entity = new CohortEntity
            {
                Id = 1,
                Name = "Test Cohort",
                CourseCode = "CS101",
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.CourseCode, result.CourseCode);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Cohort_To_CohortEntity()
        {
            var model = new Cohort
            {
                Id = 1,
                Name = "Test Cohort",
                CourseCode = "CS101",
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.CourseCode, result.CourseCode);
        }
    }
}
