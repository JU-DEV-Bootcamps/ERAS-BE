using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class UserPollsConfiguration : IEntityTypeConfiguration<UserPollsEntity>
{
    public void Configure(EntityTypeBuilder<UserPollsEntity> Builder)
    {
        Builder.ToTable("userPolls");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<UserPollsEntity> Builder)
    {
        Builder.HasKey(Up => Up.Id);

        Builder.Property(Up => Up.UserId)
            .IsRequired()
            .HasMaxLength(100);
    }

    private static void ConfigureRelationShips(EntityTypeBuilder<UserPollsEntity> Builder)
    {
        Builder.HasOne(Up => Up.Poll)
            .WithMany()
            .HasForeignKey(Up => Up.PollId)
            .OnDelete(DeleteBehavior.Cascade);

        Builder.HasOne(Up => Up.Configuration)
            .WithMany()
            .HasForeignKey(Up => Up.ConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}