using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class VariableConfiguration : IEntityTypeConfiguration<VariableEntity>
    {
        public void Configure(EntityTypeBuilder<VariableEntity> builder)
        {
            builder.ToTable("variables");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<VariableEntity> builder)
        {
            builder.HasKey(variable => variable.Id);
            builder.Property(variable => variable.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(variable => variable.ComponentId)
                .HasColumnName("component_id")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<VariableEntity> builder)
        {
            builder.HasOne(variable => variable.Component)
                .WithMany(component => component.Variables)
                .HasForeignKey(variable => variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}