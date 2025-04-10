using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class PollConfiguration : IEntityTypeConfiguration<PollEntity>
    {
        public void Configure(EntityTypeBuilder<PollEntity> builder)
        {
            builder.ToTable("polls");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollEntity> builder)
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

        private static void ConfigureRelationShips(EntityTypeBuilder<PollEntity> builder)
        {

        }
    }
}
