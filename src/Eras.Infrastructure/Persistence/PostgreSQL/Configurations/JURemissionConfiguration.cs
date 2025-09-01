using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JURemissionConfiguration : IEntityTypeConfiguration<JURemissionEntity>
{
    public void Configure(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.ToTable("ju_remissions");
        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.HasKey(r => r.Id);

        Builder.Property(r => r.SubmitterUuid)
            .HasColumnName("submitter_uuid")
            .HasMaxLength(100)
            .IsRequired();

        Builder.Property(r => r.AssignedProfessionalUuid)
            .HasColumnName("assigned_professional_uuid")
            .HasMaxLength(100)
            .IsRequired();

        Builder.Property(r => r.Comment)
            .HasColumnName("comment")
            .HasMaxLength(1000);

        Builder.Property(r => r.Date)
            .HasColumnName("date")
            .IsRequired();

        Builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasMaxLength(50)
            .IsRequired();

        Builder.Property(r => r.StudentId)
            .HasColumnName("student_id")
            .IsRequired();

        Builder.Property(r => r.JUServiceId)
            .HasColumnName("ju_service_id")
            .IsRequired();
    }
    private void ConfigureRelationShips(EntityTypeBuilder<JURemissionEntity> Builder)
    {
        Builder.HasOne(r => r.Student)
            .WithMany()
            .HasForeignKey(r => r.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        Builder.HasOne(r => r.JUService)
            .WithMany()
            .HasForeignKey(r => r.JUServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
