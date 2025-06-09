using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollInstanceConfiguration : IEntityTypeConfiguration<PollInstanceEntity>
    {
        public void Configure(EntityTypeBuilder<PollInstanceEntity> Builder)
        {
            Builder.ToTable("poll_instances");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollInstanceEntity> Builder)
        {
            Builder.HasKey(PollInstance => PollInstance.Id);
            Builder.Property(PollInstance => PollInstance.Uuid)
                .HasColumnName("uuid")
                .HasMaxLength(50)
                .IsRequired();
            Builder.Property(Poll => Poll.LastVersion)
                .HasColumnName("last_version")
                .HasColumnType("smallint")
                .IsRequired();
            Builder.Property(Poll => Poll.LastVersionDate)
                .HasColumnName("last_version_date")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollInstanceEntity> Builder)
        {
            Builder.HasMany(PollInstance => PollInstance.Answers)
                .WithOne(Answer => Answer.PollInstance)
                .HasForeignKey(Answer => Answer.PollInstanceId);

            Builder.HasOne(PollInstance => PollInstance.Student)
                .WithMany(Student => Student.PollInstances)
                .HasForeignKey(PollInstance => PollInstance.StudentId);
        }
    }
}
