using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JUProfessionalConfiguration : IEntityTypeConfiguration<JUProfessionalEntity>
{
    public void Configure(EntityTypeBuilder<JUProfessionalEntity> Builder)
    {
        Builder.ToTable("ju_professionals");
        ConfigureColumns(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JUProfessionalEntity> Builder)
    {
        Builder.HasKey(P => P.Id);

        Builder.Property(P => P.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        Builder.Property(P => P.Uuid)
            .HasColumnName("uuid")
            .HasMaxLength(100)
            .IsRequired();
    }
}
