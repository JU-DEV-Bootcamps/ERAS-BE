using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations;
public class JUInterventionConfiguration : IEntityTypeConfiguration<JUInterventionEntity>
{
    public void Configure(EntityTypeBuilder<JUInterventionEntity> Builder)
    {
        Builder.ToTable("ju_interventions");

        ConfigureColumns(Builder);
        ConfigureRelationShips(Builder);
        AuditConfiguration.Configure(Builder);
    }

    private void ConfigureColumns(EntityTypeBuilder<JUInterventionEntity> Builder)
    {
        Builder.HasKey(I => I.Id);

        Builder.Property(I  => I.Diagnostic)
            .HasColumnName("diagnostic")
            .HasMaxLength(500);

        Builder.Property(I => I.Objective)
            .HasColumnName("objective")
            .HasMaxLength(500);

        Builder.Property(I => I.StudentId)
            .HasColumnName("student_id")
            .IsRequired();
    }
    private void ConfigureRelationShips(EntityTypeBuilder<JUInterventionEntity> Builder)
    {
        Builder.HasOne(I => I.Student)
            .WithMany()
            .HasForeignKey(I => I.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        Builder.HasMany(I => I.Remissions)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);
    }
}
