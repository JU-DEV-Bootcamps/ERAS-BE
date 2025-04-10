using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollInstanceConfiguration : IEntityTypeConfiguration<PollInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<PollInstanceEntity> builder)
        {
            builder.ToTable("poll_instances");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollInstanceEntity> builder)
        {
            builder.HasKey(pollInstance => pollInstance.Id);
            builder.Property(pollInstance => pollInstance.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollInstanceEntity> builder)
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
