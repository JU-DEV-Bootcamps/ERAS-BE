using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollConfiguration : IEntityTypeConfiguration<Poll>
    {
        public void Configure(EntityTypeBuilder<Poll> builder)
        {
            builder.ToTable("polls");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<Poll> builder)
        {
            builder.HasKey(poll => poll.Id);
            builder.Property(poll => poll.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(poll => poll.Version)
                .HasColumnName("version")
                .IsRequired();
            builder.Property(poll => poll.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<Poll> builder)
        {
            builder.HasMany(poll => poll.Variables)
                .WithMany(variable => variable.Polls)
                .UsingEntity<Dictionary<string, object>>(
                    "poll_variable",
                    join => join
                        .HasOne<Variable>()
                        .WithMany()
                        .HasForeignKey("variable_id"),
                    join => join
                        .HasOne<Poll>()
                        .WithMany()
                        .HasForeignKey("poll_id"),
                    join =>
                    {
                        join.HasKey("poll_id", "variable_id");
                    }
                );
        }
    }
}