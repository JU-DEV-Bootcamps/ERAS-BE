using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;

public static class JsonCollectionConfiguration
{
    public static PropertyBuilder<IReadOnlyCollection<Guid>> ConfigureGuidCollection(
    PropertyBuilder<IReadOnlyCollection<Guid>> builder,
    string columnType = "jsonb")
    {
        var comparer = new ValueComparer<IReadOnlyCollection<Guid>>(
        (left, right) =>
        ReferenceEquals(left, right) ||
        (left != null && right != null && left.SequenceEqual(right)),
        value => value.Aggregate(0, (current, item) => HashCode.Combine(current, item.GetHashCode())),
        value => value.ToArray()); 

        builder
            .HasColumnType(columnType)
            .HasConversion(
                valueToInsert => JsonSerializer.Serialize(valueToInsert, (JsonSerializerOptions?)null),
                valueToReturn => JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(valueToReturn, (JsonSerializerOptions?)null)
                    ?? Array.Empty<Guid>());

        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }

    public static PropertyBuilder<IReadOnlyCollection<string>> ConfigureStringCollection(
    PropertyBuilder<IReadOnlyCollection<string>> builder,
    string columnType = "jsonb")
    {
        var comparer = new ValueComparer<IReadOnlyCollection<string>>(
            (left, right) =>
                left != null &&
                right != null &&
                left.SequenceEqual(right),
            value => value.Aggregate(0, (current, item) => item == null ? HashCode.Combine(current, 0) : HashCode.Combine(current, item.GetHashCode())),
            value => value.ToArray());

        builder
            .HasColumnType(columnType)
            .HasConversion(
                valueToInsert => JsonSerializer.Serialize(valueToInsert, (JsonSerializerOptions?)null),
                valueToReturn => JsonSerializer.Deserialize<IReadOnlyCollection<string>>(valueToReturn, (JsonSerializerOptions?)null)
                    ?? Array.Empty<string>());

        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }
}
