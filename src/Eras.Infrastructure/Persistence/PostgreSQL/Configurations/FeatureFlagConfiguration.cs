using Eras.Domain.Entities.FeatureFlagManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> Builder)
    {
        Builder.ToTable("feature_flag");
        ConfigureColumns(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<FeatureFlag> Builder)
    {
        Builder.HasKey(FeatureFlag => FeatureFlag.Id);
        Builder.Property(FeatureFlag => FeatureFlag.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();
        Builder.Property(FeatureFlag => FeatureFlag.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
        Builder.HasIndex(FeatureFlag => FeatureFlag.Name)
            .IsUnique()
            .HasDatabaseName("idx_feature_flag_name");
        Builder.Property(FeatureFlag => FeatureFlag.Description)
            .HasColumnName("description")
            .HasDefaultValue(string.Empty)
            .IsRequired();
        Builder.Property(FeatureFlag => FeatureFlag.IsEnabled)
            .HasColumnName("is_enabled")
            .HasDefaultValue(false)
            .IsRequired();
    }
}