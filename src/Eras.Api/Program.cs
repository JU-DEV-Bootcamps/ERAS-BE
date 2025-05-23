using Serilog;
using Serilog.Events;
using Eras.Application.Services;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Eras.Api;
using Eras.Infrastructure;
using Eras.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Eras.Infrastructure.External.CosmicLatteClient;


var builder = WebApplication.CreateBuilder(args);


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

builder.Services.AddHealthChecks()                          
    .AddCheck<CosmicLatteHealthCheck>("cosmicLatteApi");  
                                                            
var app = builder.Build();

// Automitcally log HTTP requests
app.UseSerilogRequestLogging();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // Read the view definition
    var sqlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                   "Persistence/PostgreSQL/Views/vErasCalculationByPoll.sql");

    if (!File.Exists(sqlFilePath))
    {
        throw new FileNotFoundException($"SQL File not found: {sqlFilePath}");
    }

    var sqlScript = File.ReadAllText(sqlFilePath);
    // Execute the SQL query
    using (var connection = dbContext.Database.GetDbConnection())
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlScript;
            command.ExecuteNonQuery();
        }
    }

}

// Enable CORS
app.UseCors("CORSPolicy");

app.MapHealthChecks("/api/v1/health", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// To handle all the exceptions in the API
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.Run();
