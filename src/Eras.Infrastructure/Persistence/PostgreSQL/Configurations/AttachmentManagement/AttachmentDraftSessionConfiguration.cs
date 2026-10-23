using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AttachmentManagement;

public sealed class AttachmentDraftSessionConfiguration : IEntityTypeConfiguration<AttachmentDraftSessionEntity>
{
    public void Configure(EntityTypeBuilder<AttachmentDraftSessionEntity> builder)
    {
        builder.ToTable("attachment_draft_sessions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        builder.Property(x => x.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(
                valueToInsert => valueToInsert.ToUniversalTime(),
                valueToReturn => DateTime.SpecifyKind(valueToReturn, DateTimeKind.Utc))
            .IsRequired();
    }
}
