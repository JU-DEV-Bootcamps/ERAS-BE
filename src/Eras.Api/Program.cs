using Serilog;
using Serilog.Events;
using Eras.Application.Services;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Eras.Api;
using Eras.Infrastructure;
using Eras.Api.Middleware;


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

var app = builder.Build();

// Automitcally log HTTP requests
app.UseSerilogRequestLogging();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContextDrop = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Read the view definition
    var sqlFilePathDrop = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                   "Persistence/PostgreSQL/Views/vErasCalculationByPollDrop.sql");

    if (!File.Exists(sqlFilePathDrop))
    {
        throw new FileNotFoundException($"SQL File not found: {sqlFilePathDrop}");
    }

    var sqlScriptDrop = File.ReadAllText(sqlFilePathDrop);
    // Execute the SQL query
    using (var connection = dbContextDrop.Database.GetDbConnection())
    {
        connection.Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = sqlScriptDrop;
            command.ExecuteNonQuery();
        }
    }
}
using (var scope = app.Services.CreateScope())
{
    using (var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
    {
        dbContext.Database.Migrate();


        // Read the view definition
        var sqlFilePathUp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                       "Persistence/PostgreSQL/Views/vErasCalculationByPoll.sql");

        if (!File.Exists(sqlFilePathUp))
        {
            throw new FileNotFoundException($"SQL File not found: {sqlFilePathUp}");
        }

        var sqlScript = File.ReadAllText(sqlFilePathUp);
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

}

// Enable CORS
app.UseCors("CORSPolicy");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(_ =>
        {
            _.EnableDeepLinking();
            _.OAuthClientId("api-client");
            _.OAuthAppName("Swagger new ");
        }
        );

app.UseHsts();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// To handle all the exceptions in the API
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();
app.Run();
