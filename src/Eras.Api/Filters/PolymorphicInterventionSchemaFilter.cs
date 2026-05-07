using Eras.Application.DTOs.AssessmentManagement;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace Eras.Api.Filters;

[ExcludeFromCodeCoverage]
public class PolymorphicInterventionSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(InterventionDto))
        {
            schema.Properties["kind"] = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny>
                {
                    new OpenApiString("Individual"),
                    new OpenApiString("Group")
                }
            };

            schema.Properties["area"] = new OpenApiSchema { Type = "string", Nullable = true };
            schema.Properties["participantIds"] = new OpenApiSchema
            {
                Type = "array",
                Items = new OpenApiSchema { Type = "string", Format = "uuid" }
            };

            schema.Example = new OpenApiObject
            {
                ["kind"] = new OpenApiString("Individual or Group"),
                ["dateUtc"] = new OpenApiString("2026-05-06T10:00:00Z"),
                ["activityType"] = new OpenApiString("string"),
                ["professional"] = new OpenApiString("string"),
                ["comments"] = new OpenApiString("string"),
                ["attachments"] = new OpenApiArray(),
                ["area"] = new OpenApiString("(Group only) string"),
                ["participantIds"] = new OpenApiArray
                {
                    new OpenApiString("(Group only) 7f317802-fb3e-4d95-8c87-3b39ff7cc604")
                }
            };
        }

        if (context.Type == typeof(AddInterventionDto))
        {
            schema.Example = new OpenApiObject
            {
                ["assessmentId"] = new OpenApiString("5fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ["intervention"] = new OpenApiObject
                {
                    ["kind"] = new OpenApiString("Individual or Group"),
                    ["dateUtc"] = new OpenApiString("2026-05-06T10:00:00Z"),
                    ["activityType"] = new OpenApiString("string"),
                    ["professional"] = new OpenApiString("string"),
                    ["comments"] = new OpenApiString("string"),
                    ["attachments"] = new OpenApiArray(),
                    ["area"] = new OpenApiString("(Group only) string"),
                    ["participantIds"] = new OpenApiArray
                    {
                        new OpenApiString("(Group only) 7f317802-fb3e-4d95-8c87-3b39ff7cc604")
                    }
                }
            };
        }
    }
}