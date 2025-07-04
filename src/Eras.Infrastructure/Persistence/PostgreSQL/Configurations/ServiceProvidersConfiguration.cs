using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class ServiceProvidersConfiguration : IEntityTypeConfiguration<ServiceProvidersEntity>
{
    public void Configure(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.ToTable("serviceProviders");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.HasKey(Student => Student.Id);
        Builder.Property(Sp => Sp.ServiceProviderName)
            .IsRequired()
            .HasMaxLength(255);
        Builder.Property(Sp => Sp.ServiceProviderLogo)
            .IsRequired();
    }

    private static void ConfigureRelationShips(EntityTypeBuilder<ServiceProvidersEntity> Builder)
    {
        Builder.HasMany(Sp => Sp.Configurations)
            .WithOne(C => C.ServiceProvider)
            .HasForeignKey(C => C.ServiceProviderId);
    }
}
