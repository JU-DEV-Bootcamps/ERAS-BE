using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class ComponentConfiguration : IEntityTypeConfiguration<ComponentEntity>
    {
        public void Configure(EntityTypeBuilder<ComponentEntity> Builder)
        {
            Builder.ToTable("components");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<ComponentEntity> Builder)
        {
            Builder.HasKey(Component => Component.Id);
            Builder.Property(Component => Component.Name)
                .HasColumnName("name")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<ComponentEntity> Builder)
        {
            Builder.HasMany(Component => Component.Variables)
                .WithOne(Variable => Variable.Component)
                .HasForeignKey(Variable => Variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
