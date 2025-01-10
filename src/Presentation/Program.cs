using Entities;
using Infrastructure.CosmicLatteClient.CosmicLatteClient;
using Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://staging.cosmic-latte.com/api/1.0/"); // configuration["CosmicLatte:URL"];
builder.Services.AddSingleton(httpClient);

builder.Services.AddSingleton<ICosmicLatteAPIService<CosmicLatteStatus>, CosmicLatteAPIService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

    
app.MapControllers();

app.Run();
