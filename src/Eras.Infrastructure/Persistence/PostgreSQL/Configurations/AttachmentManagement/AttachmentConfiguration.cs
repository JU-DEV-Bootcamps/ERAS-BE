using Eras.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AttachmentManagement;

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("attachments");

        ConfigureColumns(builder);

        builder.HasIndex(a => new { a.EntityType, a.EntityId })
            .HasDatabaseName("idx_attachments_entity_type_entity_id");
    }

    private static void ConfigureColumns(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        builder.Property(a => a.EntityType)
            .HasColumnName("entity_type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.EntityId)
            .HasColumnName("entity_id")
            .IsRequired();

        builder.Property(a => a.OriginalFileName)
            .HasColumnName("original_file_name")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.StorageKey)
            .HasColumnName("storage_key")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(a => a.StorageProvider)
            .HasColumnName("storage_provider")
            .HasMaxLength(50)
            .HasConversion<string>()
            .HasDefaultValue(AttachmentStorageProvider.LocalFileSystem)
            .IsRequired();

        builder.Property(a => a.MimeType)
            .HasColumnName("mime_type")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.SizeBytes)
            .HasColumnName("size_bytes")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(a => a.ContentHash)
            .HasColumnName("content_hash")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(
                valueToInsert => valueToInsert.ToUniversalTime(),
                valueToReturn => DateTime.SpecifyKind(valueToReturn, DateTimeKind.Utc))
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.StorageRelocationPendingAt)
            .HasColumnName("storage_relocation_pending_at")
            .HasConversion(
                valueToInsert => valueToInsert.HasValue ? valueToInsert.Value.ToUniversalTime() : (DateTime?)null,
                valueToReturn => valueToReturn.HasValue ? DateTime.SpecifyKind(valueToReturn.Value, DateTimeKind.Utc) : (DateTime?)null)
            .IsRequired(false);
    }
}
