using Eras.Application.Services;
using Eras.Infrastructure.External.CosmicLatteClient;
using Eras.Infrastructure.External.KeycloakClient;
using Eras.Infrastructure.Persistence.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Eras.Application.Contracts.Infrastructure;
using Eras.Application.Contracts.Persistence;
using Eras.Infrastructure.Persistence.PostgreSQL.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<ICosmicLatteAPIService, CosmicLatteAPIService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"{builder.Configuration["Keycloak:BaseUrl"]}/realms/{builder.Configuration["Keycloak:Realm"]}",

        ValidateAudience = true,
        ValidAudience = "account",

        ValidateIssuerSigningKey = true,
        ValidateLifetime = false,

        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            var client = new HttpClient();
            var keyUri = $"{parameters.ValidIssuer}/protocol/openid-connect/certs";
            var response = client.GetAsync(keyUri).Result;
            var keys = new JsonWebKeySet(response.Content.ReadAsStringAsync().Result);

            return keys.GetSigningKeys();
        }
    };

    options.RequireHttpsMetadata = false; // Only in develop environment
    options.SaveToken = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("ErasConnection"));
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));

});

builder.Services.AddPersistenceServices();

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

builder.Services.AddScoped<KeycloakAuthService>();
//builder.Services.AddScoped<IStudentRepository<Student>, StudentRepository>();
//builder.Services.AddScoped<IStudentService, StudentService>();

// Add the StudentService to the dependency injection container
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IPollService, PollService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddApplicationServices();


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
