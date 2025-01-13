using Entities;
using Infrastructure.CosmicLatteClient.CosmicLatteClient;
using Services;
using Infrastructure.Persistence.PostgreSQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var httpClient = new HttpClient();
string cosmicUrl = builder.Configuration.GetSection("CosmicLatteUrl").Value;

httpClient.BaseAddress = new Uri(cosmicUrl); 
builder.Services.AddSingleton(httpClient);

builder.Services.AddSingleton<ICosmicLatteAPIService<CosmicLatteStatus>, CosmicLatteAPIService>();


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
