using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class VariableConfiguration : IEntityTypeConfiguration<Variable>
    {
        public void Configure(EntityTypeBuilder<Variable> builder)
        {
            builder.ToTable("variables");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private void ConfigureColumns(EntityTypeBuilder<Variable> builder)
        {
            builder.HasKey(variable => variable.Id);
            builder.Property(variable => variable.Name)
                .HasColumnName("name")
                .IsRequired();
        }

        private void ConfigureRelationShips(EntityTypeBuilder<Variable> builder)
        {
            builder.HasOne(variable => variable.Component)
                .WithMany(component => component.Variables)
                .HasForeignKey(variable => variable.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(variable => variable.Polls)
                .WithMany(poll => poll.Variables)
                .UsingEntity<Dictionary<string, object>>(
                    "poll_variable",
                    join => join
                        .HasOne<Poll>()
                        .WithMany()
                        .HasForeignKey("poll_id"),
                    join => join
                        .HasOne<Variable>()
                        .WithMany()
                        .HasForeignKey("variable_id"),
                    join =>
                    {
                        join.HasKey("poll_id", "variable_id");
                    }
                );
        }
    }
}