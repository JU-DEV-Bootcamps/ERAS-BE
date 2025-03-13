using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Mappers;


namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Mappers
{
    public class ComponentMapperTest
    {
        [Fact]
        public void ToDomain_Should_Convert_ComponentEntity_To_Component()
        {
            var entity = new ComponentEntity
            {
                Id = 1,
                Name = "Test Component",
            };
            var result = entity.ToDomain();
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Audit, result.Audit);
        }

        [Fact]
        public void ToPersistence_Should_Convert_Component_To_ComponentEntity()
        {
            var model = new Component
            {
                Id = 1,
                Name = "Test Component",
            };
            var result = model.ToPersistence();
            Assert.NotNull(result);
            Assert.Equal(model.Id, result.Id);
            Assert.Equal(model.Name, result.Name);
            Assert.Equal(model.Audit, result.Audit);
        }
    }
}
