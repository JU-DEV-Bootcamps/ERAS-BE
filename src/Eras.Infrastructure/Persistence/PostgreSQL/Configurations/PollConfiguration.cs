using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollConfiguration : IEntityTypeConfiguration<PollEntity>
    {
        public void Configure(EntityTypeBuilder<PollEntity> Builder)
        {
            Builder.ToTable("polls");

            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollEntity> Builder)
        {
            Builder.HasKey(Poll => Poll.Id);
            Builder.Property(Poll => Poll.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
            Builder.Property(Poll => Poll.Uuid)
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
            Builder.Property(Poll => Poll.ParentId)
                .HasColumnName("parent_id")
                .HasMaxLength(100)
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollEntity> Builder)
        {

        }
    }
}
