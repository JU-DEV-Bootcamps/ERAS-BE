using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JUServicesConfiguration : IEntityTypeConfiguration<JUServiceEntity>
{
    public void Configure(EntityTypeBuilder<JUServiceEntity> Builder)
    {
        Builder.ToTable("ju_services");

        ConfigureColumns(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JUServiceEntity> Builder)
    {
        Builder.HasKey(S => S.Id);

        Builder.Property(S => S.Name)
            .HasColumnName("service_name")
            .HasMaxLength(100)
            .IsRequired();
    }
}

