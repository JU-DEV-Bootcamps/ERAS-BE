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
                .IsRequired();
            Builder.Property(Poll => Poll.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollEntity> Builder)
        {

        }
    }
}