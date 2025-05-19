using Eras.Domain.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations
{
    public static class VersionConfiguration
    {
        public static void Configure<T>(EntityTypeBuilder<T> Builder)
            where T : class, IVersionableEntity
        {
            Builder.OwnsOne(Entity => Entity.Version, Vi =>
            {
                Vi.Property(A => A.VersionNumber)
                    .HasColumnName("version_number")
                    .IsRequired();

                Vi.Property(A => A.VersionDate)
                    .HasColumnName("version_date")
                    .HasConversion(
                        ValueToInsert => ValueToInsert.ToUniversalTime(),
                        ValueToReturn => DateTime.SpecifyKind(
                            ValueToReturn,
                            DateTimeKind.Utc
                        )
                    )
                    .IsRequired();
            });
        }
    }
}
