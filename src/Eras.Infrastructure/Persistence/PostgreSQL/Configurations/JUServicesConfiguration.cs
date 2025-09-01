using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JUServicesConfiguration : IEntityTypeConfiguration<JUServicesEntity>
{
    public void Configure(EntityTypeBuilder<JUServicesEntity> Builder)
    {
        Builder.ToTable("ju_services");

        ConfigureColumns(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JUServicesEntity> Builder)
    {
        Builder.HasKey(S => S.Id);

        Builder.Property(S => S.ServiceName)
            .HasColumnName("service_name")
            .HasMaxLength(100)
            .IsRequired();
    }
}

