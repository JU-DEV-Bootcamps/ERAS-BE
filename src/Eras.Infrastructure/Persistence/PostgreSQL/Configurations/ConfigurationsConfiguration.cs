using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class ConfigurationsConfiguration : IEntityTypeConfiguration<ConfigurationsEntity>
{
    public void Configure(EntityTypeBuilder<ConfigurationsEntity> Builder)
    {
        Builder.ToTable("configurations");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<ConfigurationsEntity> Builder)
    {
        Builder.HasKey(C => C.Id);
        
        Builder.Property(C => C.UserId)
            .IsRequired()
            .HasMaxLength(100);

        Builder.Property(C => C.BaseURL)
            .IsRequired()
            .HasMaxLength(500);

        Builder.Property(C => C.EncryptedKey)
            .IsRequired();
    }

    private static void ConfigureRelationShips(EntityTypeBuilder<ConfigurationsEntity> Builder)
    {
        Builder.HasOne(C => C.ServiceProvider)
            .WithMany(Sp => Sp.Configurations)
            .HasForeignKey(C => C.ServiceProviderId);
    }
}
