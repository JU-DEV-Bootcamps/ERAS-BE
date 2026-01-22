using Eras.Api;
using Eras.Api.Filters;
using Eras.Api.Middleware;
using Eras.Application.Services;
using Eras.Infrastructure;
using Eras.Infrastructure.Persistence.PostgreSQL;

using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(
    options => options.Filters.Add<ErrorFilter>());

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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var dropsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence/PostgreSQL/Views/Drops");

    if (Directory.Exists(dropsPath))
    {
        var files = Directory.GetFiles(dropsPath, "*.sql").OrderBy(f => f);
        using (var connection = dbContext.Database.GetDbConnection())
        {
            connection.Open();
            foreach (var file in files)
            {
                var sql = File.ReadAllText(file);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

// Start migrations and views Creation
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    var upsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Persistence/PostgreSQL/Views/Ups");

    if (Directory.Exists(upsPath))
    {
        var files = Directory.GetFiles(upsPath, "*.sql").OrderBy(f => f);
        using (var connection = dbContext.Database.GetDbConnection())
        {
            connection.Open();
            foreach (var file in files)
            {
                var sql = File.ReadAllText(file);
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
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
