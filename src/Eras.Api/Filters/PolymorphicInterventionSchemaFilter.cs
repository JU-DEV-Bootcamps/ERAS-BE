using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;
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
                    ["1"] = new OpenApiBoolean(true)
                },
                ["mode"] = new OpenApiString("InPlace"),
                ["status"] = new OpenApiString("Created"),
                ["remarks"] = new OpenApiString("El estudiante mostró avance."),
                ["attachments"] = new OpenApiArray()
            };
        }

        if (context.Type == typeof(AddInterventionDto))
        {
            schema.Example = new OpenApiObject
            {
                ["assessmentId"] = new OpenApiInteger(1),
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
                        ["1"] = new OpenApiBoolean(true)
                    },
                    ["mode"] = new OpenApiString("InPlace"),
                    ["remarks"] = new OpenApiString("El estudiante mostró avance."),
                    ["attachments"] = new OpenApiArray()
                }
            };
        }
    }
}