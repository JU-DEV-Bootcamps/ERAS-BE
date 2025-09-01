using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JUInterventionsConfiguration : IEntityTypeConfiguration<JUInterventionsEntity>
{
    public void Configure(EntityTypeBuilder<JUInterventionsEntity> Builder)
    {
        Builder.ToTable("ju_interventions");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JUInterventionsEntity> Builder)
    {
        Builder.HasKey(i => i.Id);

        Builder.Property(i => i.Diagnostic)
            .HasColumnName("diagnostic")
            .HasMaxLength(500);

        Builder.Property(i => i.Objective)
            .HasColumnName("objective")
            .HasMaxLength(500);

        Builder.Property(i => i.StudentId)
            .HasColumnName("student_id")
            .IsRequired();
    }
    private void ConfigureRelationShips(EntityTypeBuilder<JUInterventionsEntity> Builder)
    {
        Builder.HasOne(i => i.Student)
            .WithMany()
            .HasForeignKey(i => i.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        Builder.HasMany(i => i.RemissionsList)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
