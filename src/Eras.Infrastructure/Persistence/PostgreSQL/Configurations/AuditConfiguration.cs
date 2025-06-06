using Eras.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public static class AuditConfiguration
    {
        public static void Configure<T>(EntityTypeBuilder<T> Builder)
            where T : class, IAuditableEntity
        {
            Builder.OwnsOne(Entity => Entity.Audit, Audit =>
            {
                Audit.Property(A => A.CreatedBy)
                    .HasColumnName("created_by")
                    .HasMaxLength(50)
                    .IsRequired();

                Audit.Property(A => A.ModifiedBy)
                    .HasColumnName("modified_by")
                    .HasMaxLength(50)
                    .IsRequired(false);

                Audit.Property(A => A.CreatedAt)
                    .HasColumnName("created_at")
                    .HasConversion(
                        ValueToInsert => ValueToInsert.ToUniversalTime(),
                        ValueToReturn => DateTime.SpecifyKind(
                            ValueToReturn,
                            DateTimeKind.Utc
                        )
                    )
                    .IsRequired();

                Audit.Property(A => A.ModifiedAt)
                    .HasColumnName("updated_at")
                    .HasConversion(
                        ValueToInsert => ValueToInsert.HasValue
                            ? ValueToInsert.Value.ToUniversalTime()
                            : (DateTime?)null,
                        ValueToReturn => ValueToReturn.HasValue
                            ? DateTime.SpecifyKind(
                                ValueToReturn.Value,
                                DateTimeKind.Utc
                            )
                            : (DateTime?)null
                    )
                    .IsRequired(false);
            });
        }
    }
}
