using System.Text.Json.Serialization;

using Eras.Api;
using Eras.Api.Filters;
using Eras.Api.Middleware;
using Eras.Application.Contracts.Services;
using Eras.Application.Services;
using Eras.Infrastructure;
using Eras.Infrastructure.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(
    options => options.Filters.Add<ErrorFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
        .Add(new JsonStringEnumConverter());
    });

LogEventLevel minimumLevel = builder.Environment.IsDevelopment()
    ? LogEventLevel.Debug
    : LogEventLevel.Warning;

// Logging configuration
var logger = new LoggerConfiguration()
    .MinimumLevel.Is(minimumLevel)
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.log",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information
    )
    .CreateLogger();

builder.Host.UseSerilog(logger);

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

// Automitcally log HTTP requests
app.UseSerilogRequestLogging();

// Apply schema migrations and recreate the SQL views that depend on them — IDataBase.MigrateAsync,
// implemented by AppDbContext (src/Eras.Infrastructure/Persistence/PostgreSQL/AppDbContext.cs).
// Previously three near-identical inline blocks living directly in this file (drop views, migrate,
// recreate views, each manually opening/closing a raw DbConnection); consolidated into the
// DbContext itself since "bring the schema and its views up to date" is squarely that class's own
// responsibility, not something Program.cs should be doing by hand.
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IDataBase>();
    await database.MigrateAsync();
}

// Legacy Intervention attachment data migration (Attachments Refactor) — kept out of Program.cs
// itself; see IInterventionAttachmentMigrationStartupTask for why (completion-marker gating,
// logging, deciding when to mark done) and IInterventionAttachmentMigrationService for the
// migration algorithm itself.
using (var migrationScope = app.Services.CreateScope())
{
    var migrationTask = migrationScope.ServiceProvider.GetRequiredService<IInterventionAttachmentMigrationStartupTask>();
    await migrationTask.RunAsync();
}

// Enable CORS
app.UseCors("CORSPolicy");

// Enable Swagger just for development environments.
bool isSwaggerEnabled = builder.Configuration["EnableSwagger"] == "true";
if (isSwaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(_ =>
            {
                _.EnableDeepLinking();
                _.OAuthClientId("api-client");
                _.OAuthAppName("Swagger new ");
            }
            );
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// To handle all the exceptions in the API
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.Run();
