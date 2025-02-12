using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class ComponentConfiguration : IEntityTypeConfiguration<ComponentEntity>
    {
        public void Configure(EntityTypeBuilder<ComponentEntity> builder)
        {
            builder.ToTable("components");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<ComponentEntity> builder)
        {
            builder.HasKey(component => component.Id);
            builder.Property(component => component.Name)
                .HasColumnName("name")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<ComponentEntity> builder)
        {
            builder.HasMany(component => component.Variables)
                .WithOne(variable => variable.Component)
                .HasForeignKey(variable => variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}