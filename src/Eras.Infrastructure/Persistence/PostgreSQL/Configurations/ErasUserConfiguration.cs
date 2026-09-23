using Eras.Domain.Entities.UserManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class ErasUserConfiguration : IEntityTypeConfiguration<ErasUser>
{
    public void Configure(EntityTypeBuilder<ErasUser> Builder)
    {
        Builder.ToTable("eras_users");
        ConfigureColumns(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<ErasUser> Builder)
    {
        Builder.HasKey(entity => entity.Id);
        Builder.HasIndex(entity => entity.Email)
            .IsUnique()
            .HasDatabaseName("idx_eras_user_email");

        Builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(entity => entity.Sub)
            .HasColumnName("sub")
            .HasMaxLength(36);

        Builder.Property(entity => entity.Email)
            .HasColumnName("email")
            .IsRequired();

        Builder.Property(entity => entity.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty)
            .IsRequired();
        
        Builder.Property(entity => entity.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(100)
            .HasDefaultValue(string.Empty)
            .IsRequired();
        
        Builder.Property(entity => entity.Role)
            .HasColumnName("role")
            .HasDefaultValue(ErasRole.Guest.Label)
            .IsRequired();
        
        Builder.Property(entity => entity.IsSynced)
            .HasColumnName("is_synced")
            .HasDefaultValue(false)
            .IsRequired();
    }
}