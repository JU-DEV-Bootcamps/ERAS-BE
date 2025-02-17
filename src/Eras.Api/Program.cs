using Serilog;
using Serilog.Events;
using Eras.Application.Services;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Eras.Api;
using Eras.Infrastructure;
using Eras.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Logging configuration
var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
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
    dbContext.Database.Migrate();
}

// Enable CORS
app.UseCors("CORSPolicy");

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
