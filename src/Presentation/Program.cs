using ERAS.Application.Services;
using ERAS.Infrastructure.External.CosmicLatteClient;
using ERAS.Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICosmicLatteAPIService, CosmicLatteAPIService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ErasConnection")));

builder.Services.AddCors(o =>
{
    o.AddPolicy("CORSPolicy", policy =>
    {
        string allowedHosts = builder.Configuration["AllowedHosts"] ?? "*";
        policy.WithOrigins(allowedHosts)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
    });
});

var app = builder.Build();

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

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
