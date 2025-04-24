using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class VariableConfiguration : IEntityTypeConfiguration<VariableEntity>
    {
        public void Configure(EntityTypeBuilder<VariableEntity> Builder)
        {
            Builder.ToTable("variables");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<VariableEntity> Builder)
        {
            Builder.HasKey(Variable => Variable.Id);
            Builder.Property(Variable => Variable.Name)
                .HasColumnName("name")
                .IsRequired();
            Builder.Property(Variable => Variable.ComponentId)
                .HasColumnName("component_id")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<VariableEntity> Builder)
        {
            Builder.HasOne(Variable => Variable.Component)
                .WithMany(Component => Component.Variables)
                .HasForeignKey(Variable => Variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
