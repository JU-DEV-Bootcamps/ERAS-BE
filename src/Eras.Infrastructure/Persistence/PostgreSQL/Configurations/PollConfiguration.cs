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
            
            builder.OwnsOne(poll => poll.Audit, audit =>
            {
                audit.Property(a => a.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired();
                audit.Property(a => a.ModifiedAt)
                    .HasColumnName("updated_at")
                    .IsRequired(false);
            });

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