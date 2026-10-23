using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public static class UtcDateTimeConfiguration
{
    public static PropertyBuilder<DateTime> Configure(
        PropertyBuilder<DateTime> builder)
    {
        return builder.HasConversion(
            valueToInsert => valueToInsert.ToUniversalTime(),
            valueToReturn => DateTime.SpecifyKind(valueToReturn, DateTimeKind.Utc));
    }

    public static PropertyBuilder<DateTime?> ConfigureNullable(
        PropertyBuilder<DateTime?> builder)
    {
        return builder.HasConversion(
            valueToInsert => valueToInsert.HasValue
                ? valueToInsert.Value.ToUniversalTime()
                : (DateTime?)null,
            valueToReturn => valueToReturn.HasValue
                ? DateTime.SpecifyKind(valueToReturn.Value, DateTimeKind.Utc)
                : (DateTime?)null);
    }
}