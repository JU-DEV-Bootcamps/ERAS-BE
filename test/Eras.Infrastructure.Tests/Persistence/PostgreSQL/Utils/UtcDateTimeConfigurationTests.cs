using Eras.Infrastructure.Persistence.PostgreSQL.Configurations.AssessmentManagement;
using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Tests.Persistence.PostgreSQL.Utils;

public class UtcDateTimeConfigurationTests
{
    [Fact]
    public void Configure_ConvertsDateTimeToUtc()
    {
        var property = new ModelBuilder()
            .Entity<TestEntity>()
            .Property(x => x.CreatedAt);

        var configured = UtcDateTimeConfiguration.Configure(property);

        Assert.NotNull(configured);
    }

    [Fact]
    public void ConfigureNullable_ReturnsConfiguredProperty()
    {
        var modelBuilder = new ModelBuilder();

        var property = modelBuilder
            .Entity<TestEntity>()
            .Property(x => x.NullableCreatedAt);

        var configured = UtcDateTimeConfiguration.ConfigureNullable(property);

        Assert.NotNull(configured);
    }

    private class TestEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? NullableCreatedAt { get; set; }
    }

    [Fact]
    public void Configure_ConvertsLocalDateTimeToUtc()
    {
        var modelBuilder = new ModelBuilder();

        var property = modelBuilder
            .Entity<TestEntity>()
            .Property(x => x.CreatedAt);

        UtcDateTimeConfiguration.Configure(property);

        var model = modelBuilder.Model;
        var entityType = model.FindEntityType(typeof(TestEntity))!;
        var propertyMetadata = entityType.FindProperty(nameof(TestEntity.CreatedAt))!;

        var converter = propertyMetadata.GetValueConverter()!;

        var localDate = new DateTime(
            2026, 8, 14, 10, 0, 0,
            DateTimeKind.Local);

        var converted = converter.ConvertToProvider(localDate);

        Assert.Equal(
            localDate.ToUniversalTime(),
            converted);
    }

    [Fact]
    public void ConfigureNullable_WithValue_ConvertsToUtc()
    {
        var modelBuilder = new ModelBuilder();

        var property = modelBuilder
            .Entity<TestEntity>()
            .Property(x => x.NullableCreatedAt);

        UtcDateTimeConfiguration.ConfigureNullable(property);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity))!;
        var propertyMetadata =
            entityType.FindProperty(nameof(TestEntity.NullableCreatedAt))!;

        var converter = propertyMetadata.GetValueConverter()!;

        var value = new DateTime(
            2026, 8, 14, 10, 0, 0,
            DateTimeKind.Local);

        var converted = converter.ConvertToProvider(value);

        Assert.Equal(value.ToUniversalTime(), converted);
    }

    [Fact]
    public void ConfigureNullable_WithNull_ReturnsNull()
    {
        var modelBuilder = new ModelBuilder();

        var property = modelBuilder
            .Entity<TestEntity>()
            .Property(x => x.NullableCreatedAt);

        UtcDateTimeConfiguration.ConfigureNullable(property);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity))!;
        var propertyMetadata =
            entityType.FindProperty(nameof(TestEntity.NullableCreatedAt))!;

        var converter = propertyMetadata.GetValueConverter()!;

        var converted = converter.ConvertToProvider(null);

        Assert.Null(converted);
    }
}
