using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class StudentMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_StudentEntity_To_Student()
        {
            var entity = new StudentEntity
            {
                Id = 1,
                Name = "Test Student",
                Email = "test@student.com",
                Uuid = "1234",
                StudentDetail = new StudentDetailEntity()
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Email, result.Email);
            Assert.Equal(entity.Uuid, result.Uuid);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Student_To_StudentEntity()
        {
            var model = new Student
            {
                Id = 1,
                Name = "Test Student",
                Email = "test@student.com",
                Uuid = "1234",
                StudentDetail = new StudentDetail()
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.Email, result.Email);
            Assert.Equal(model.Uuid, result.Uuid);
        }
    }
}
