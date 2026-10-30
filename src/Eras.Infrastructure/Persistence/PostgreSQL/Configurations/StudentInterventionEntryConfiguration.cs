using Eras.Domain.Entities;
using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;

public sealed class StudentInterventionEntryConfiguration : IEntityTypeConfiguration<StudentInterventionEntry>
{
    public void Configure(EntityTypeBuilder<StudentInterventionEntry> Builder)
    {
        Builder.ToTable("student_intervention_entries");
        ConfigureColumns(Builder);
        ConfigureRelationships(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private static void ConfigureColumns(EntityTypeBuilder<StudentInterventionEntry> Builder)
    {
        Builder.HasKey(Entry => Entry.Id);

        Builder.Property(Entry => Entry.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd()
            .UseIdentityByDefaultColumn();

        Builder.Property(Entry => Entry.StudentId).HasColumnName("student_id").IsRequired();

        Builder.Property(Entry => Entry.Kind)
            .HasColumnName("kind")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        Builder.Property(Entry => Entry.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        Builder.Property(Entry => Entry.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
        Builder.Property(Entry => Entry.EntryDate).HasColumnName("entry_date").IsRequired();
        Builder.Property(Entry => Entry.CreatedByName).HasColumnName("created_by_name").HasMaxLength(200).IsRequired();

        Builder.Property(Entry => Entry.IsDeleted)
            .HasColumnName("is_deleted")
            .HasDefaultValue(false)
            .IsRequired();

        // A deleted entry disappears from every list but stays stored with who deleted it and when.
        Builder.HasQueryFilter(Entry => !Entry.IsDeleted);

        Builder.HasIndex(Entry => Entry.StudentId).HasDatabaseName("ix_student_intervention_entries_student_id");
    }

    private static void ConfigureRelationships(EntityTypeBuilder<StudentInterventionEntry> Builder)
    {
        Builder.HasOne<StudentEntity>()
            .WithMany()
            .HasForeignKey(Entry => Entry.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
