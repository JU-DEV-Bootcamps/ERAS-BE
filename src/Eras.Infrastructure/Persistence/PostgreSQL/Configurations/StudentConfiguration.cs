using Eras.Infrastructure.Persistence.PostgreSQL.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
    {
        public void Configure(EntityTypeBuilder<StudentEntity> Builder)
        {
            Builder.ToTable("students");
            ConfigureColumns(Builder);
            ConfigureRelationShips(Builder);
            AuditConfiguration.Configure(Builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentEntity> Builder)
        {
            Builder.HasKey(Student => Student.Id);

            Builder.Property(Student => Student.Name)
                .HasColumnName("name")
                .HasMaxLength(254)
                .IsRequired();
            Builder.Property(Student => Student.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();
            Builder.Property(Student => Student.Uuid)
                .HasColumnName("uuid")
                .HasMaxLength(50)
                .IsRequired();
            Builder.Property(Student => Student.IsImported)
                .HasColumnName("is_imported")
                .IsRequired();
        }
        private static void ConfigureRelationShips(EntityTypeBuilder<StudentEntity> Builder)
        {
            Builder.HasOne(Student => Student.StudentDetail)
                .WithOne(StudentDetail => StudentDetail.Student)
                .HasForeignKey<StudentDetailEntity>(StudentDetail => StudentDetail.StudentId);

            Builder.HasMany(Student => Student.PollInstances)
                .WithOne(PollInstance => PollInstance.Student)
                .HasForeignKey(PollInstance => PollInstance.StudentId);

            Builder.HasMany(S => S.Remissions)
                .WithMany(R => R.Students)
                .UsingEntity(J => J.ToTable("student_remissions"));
        }
    }
}
