using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class ComponentConfiguration : IEntityTypeConfiguration<Component>
    {
        public void Configure(EntityTypeBuilder<Component> builder)
        {
            builder.ToTable("components");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<Component> builder)
        {
            builder.HasKey(component => component.Id);
            builder.Property(component => component.Name)
                .HasColumnName("name")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<Component> builder)
        {
            builder.HasMany(component => component.Variables)
                .WithOne(variable => variable.Component)
                .HasForeignKey(variable => variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}