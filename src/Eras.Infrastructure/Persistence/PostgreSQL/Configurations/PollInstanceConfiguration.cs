using Eras.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollInstanceConfiguration : IEntityTypeConfiguration<PollInstance>
    {
        public void Configure(EntityTypeBuilder<PollInstance> builder)
        {
            builder.ToTable("poll_instances");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollInstance> builder)
        {
            builder.HasKey(pollInstance => pollInstance.Id);
            builder.Property(pollInstance => pollInstance.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollInstance> builder)
        {
            builder.HasMany(pollInstance => pollInstance.Answers)
                .WithOne(answer => answer.PollInstance)
                .HasForeignKey(answer => answer.PollInstanceId);
            
            builder.HasOne(pollInstance => pollInstance.Student)
                .WithMany(student => student.PollInstances)
                .HasForeignKey(pollInstance => pollInstance.StudentId);
        }
    }
}