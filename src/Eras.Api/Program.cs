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

// Apply schema migrations and recreate the SQL views that depend on them 
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IDataBase>();
    await database.MigrateAsync();
}

// Legacy Intervention attachment data migration
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

// Captures the authenticated caller into the request-scoped IUserIdentityProvider — must run
// after UseAuthentication, which is what populates HttpContext.User.
app.UseMiddleware<UserIdentityMiddleware>();

// To handle all the exceptions in the API
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.Run();
