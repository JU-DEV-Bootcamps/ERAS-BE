using Eras.Api.Filters;
using Eras.Application.DTOs.AssessmentManagement;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

using Xunit;

namespace Eras.Api.Tests.Filters
{
    public class PolymorphicInterventionSchemaFilterTests
    {
        private readonly PolymorphicInterventionSchemaFilter _filter = new();

        private static SchemaFilterContext CreateContext(Type type) =>
            new(type, null!, new SchemaRepository());

        [Fact]
        public void Apply_InterventionDto_AddsKindPropertyWithEnumValues()
        {
            // Arrange
            var schema = new OpenApiSchema();
            var context = CreateContext(typeof(InterventionDto));

            // Act
            _filter.Apply(schema, context);

            // Assert
            Assert.True(schema.Properties.ContainsKey("kind"));
            var kindSchema = schema.Properties["kind"];
            Assert.Equal("string", kindSchema.Type);
            Assert.Equal(2, kindSchema.Enum.Count);
            Assert.Contains(kindSchema.Enum, e => ((OpenApiString)e).Value == "Individual");
            Assert.Contains(kindSchema.Enum, e => ((OpenApiString)e).Value == "Group");
        }

        [Fact]
        public void Apply_InterventionDto_SetsExampleWithExpectedFields()
        {
            // Arrange
            var schema = new OpenApiSchema();
            var context = CreateContext(typeof(InterventionDto));

            // Act
            _filter.Apply(schema, context);

            // Assert
            var example = Assert.IsType<OpenApiObject>(schema.Example);
            Assert.Equal("Individual or Group", ((OpenApiString)example["kind"]).Value);
            Assert.Equal("InPlace", ((OpenApiString)example["mode"]).Value);
            Assert.True(example.ContainsKey("attendance"));
            Assert.True(example.ContainsKey("studentIds"));
        }

        [Fact]
        public void Apply_InterventionDto_PreservesExistingProperties()
        {
            // Arrange: el filtro no debería pisar propiedades ya generadas por Swashbuckle
            var schema = new OpenApiSchema();
            schema.Properties["id"] = new OpenApiSchema { Type = "integer" };
            var context = CreateContext(typeof(InterventionDto));

            // Act
            _filter.Apply(schema, context);

            // Assert
            Assert.True(schema.Properties.ContainsKey("id"));
            Assert.True(schema.Properties.ContainsKey("kind"));
        }

        [Fact]
        public void Apply_AddInterventionDto_SetsNestedExample()
        {
            // Arrange
            var schema = new OpenApiSchema();
            var context = CreateContext(typeof(AddInterventionDto));

            // Act
            _filter.Apply(schema, context);

            // Assert
            var example = Assert.IsType<OpenApiObject>(schema.Example);
            Assert.Equal(1, ((OpenApiInteger)example["assessmentId"]).Value);

            var intervention = Assert.IsType<OpenApiObject>(example["intervention"]);
            Assert.Equal("Individual or Group", ((OpenApiString)intervention["kind"]).Value);

            // AddInterventionDto no debería tocar la propiedad top-level "kind" (esa es solo de InterventionDto)
            Assert.False(schema.Properties.ContainsKey("kind"));
        }

        [Fact]
        public void Apply_UnrelatedType_DoesNothing()
        {
            // Arrange
            var schema = new OpenApiSchema();
            var context = CreateContext(typeof(string));

            // Act
            _filter.Apply(schema, context);

            // Assert
            Assert.Empty(schema.Properties);
            Assert.Null(schema.Example);
        }
    }
}