using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class DataMigrationCompletionConfiguration : IEntityTypeConfiguration<DataMigrationCompletion>
{
    public void Configure(EntityTypeBuilder<DataMigrationCompletion> builder)
    {
        builder.ToTable("data_migration_completions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("idx_data_migration_completions_name");

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at")
            .HasConversion(
                valueToInsert => valueToInsert.ToUniversalTime(),
                valueToReturn => DateTime.SpecifyKind(valueToReturn, DateTimeKind.Utc))
            .IsRequired();
    }
}
