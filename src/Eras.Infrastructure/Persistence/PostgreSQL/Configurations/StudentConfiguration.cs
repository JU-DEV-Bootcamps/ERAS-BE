using Eras.Infrastructure.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<StudentEntity>
    {
        public void Configure(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.ToTable("students");

            ConfigureColumns(builder);
            ConfigureRelationShips(builder);
            AuditConfiguration.Configure(builder);
        }

        private static void ConfigureColumns(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.HasKey(student => student.Id);
            builder.Property(student => student.Name)
                .HasColumnName("name")
                .IsRequired();
            builder.Property(student => student.Email)
                .HasColumnName("email")
                .IsRequired();
            builder.Property(student => student.Uuid)
                .HasColumnName("uuid")
                .IsRequired();
        }

        private static void ConfigureRelationShips(EntityTypeBuilder<StudentEntity> builder)
        {
            builder.HasOne(student => student.StudentDetail)
                .WithOne(studentDetail => studentDetail.Student)
                .HasForeignKey<StudentDetailEntity>(studentDetail => studentDetail.StudentId);

            builder.HasMany(student => student.PollInstances)
                .WithOne(pollInstance => pollInstance.Student)
                .HasForeignKey(pollInstance => pollInstance.StudentId);
        }
    }
}