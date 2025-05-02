using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    internal class PollVersionConfiguration : IEntityTypeConfiguration<PollVersionEntity>
    {
        public void Configure(EntityTypeBuilder<PollVersionEntity> Builder)
        {
            Builder.ToTable("poll_versions");
            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<PollVersionEntity> Builder) {
            Builder.HasKey(Pv => Pv.Id);
            Builder.Property(Pv => Pv.Name).HasColumnName("name").IsRequired();
            Builder.Property(Pv => Pv.Date).HasColumnName("date").IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<PollVersionEntity> Builder) { 
            Builder.HasOne(Pv => Pv.Poll).WithMany(Poll => Poll.PollVersions)
                .HasForeignKey(Pv => Pv.PollId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
