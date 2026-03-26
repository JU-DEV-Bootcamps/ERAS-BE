using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Domain.Entities.RemissionsManagement;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.RemissionManagement;

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