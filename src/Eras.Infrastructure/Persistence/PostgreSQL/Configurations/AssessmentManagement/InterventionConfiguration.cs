using System.Text.Json;
using Eras.Domain.Entities.AssessmentManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public sealed class InterventionConfiguration : IEntityTypeConfiguration<Intervention>
{
    public void Configure(EntityTypeBuilder<Intervention> builder)
    {
        builder.Property(e => e.Activity)
            .HasColumnName("activity")
            .HasMaxLength(200);

        builder.Property(e => e.Area)
            .HasColumnName("area")
            .HasMaxLength(200);

        builder.Property(e => e.NumberOfGuests)
            .HasColumnName("number_of_guests");

        builder.Property(e => e.NumberOfParticipants)
            .HasColumnName("number_of_participants");

        builder.Property(e => e.Professional)
            .HasColumnName("professional")
            .HasMaxLength(200);

        builder.Property(e => e.StudentIds)
            .HasColumnName("student_ids")
            .HasColumnType("integer[]");

        builder.Property(e => e.Attendance)
            .HasColumnName("attendance")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<int, bool>>(v, (JsonSerializerOptions?)null)
                     ?? new Dictionary<int, bool>()
            );

        builder.Property(e => e.Mode)
            .HasColumnName("mode")
            .HasConversion<string>();

        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasDefaultValue(InterventionStatus.Created);

        builder.Property(e => e.Remarks)
            .HasColumnName("remarks")
            .HasMaxLength(1000);

        builder.Property(e => e.Attachments)
            .HasColumnName("attachments")
            .HasColumnType("text[]");
    }
}