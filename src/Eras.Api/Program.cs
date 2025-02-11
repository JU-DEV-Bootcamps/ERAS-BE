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
using Eras.Api.Middleware;


var builder = WebApplication.CreateBuilder(args);

var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT");
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER");
var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB");

var cosmicLatteBaseUrl = Environment.GetEnvironmentVariable("COSMIC_LATTE_BASE_URL");
var cosmicLatteApiKey = Environment.GetEnvironmentVariable("COSMIC_LATTE_API_KEY");

var keycloakBaseUrl = Environment.GetEnvironmentVariable("KEYCLOAK_BASE_URL");
var keycloakRealm = Environment.GetEnvironmentVariable("KEYCLOAK_REALM");
var keycloakClientId = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID");
var keycloakClientSecret = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET");

var connectionString = $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPassword};Database={postgresDb}";

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
        ValidIssuer = $"{keycloakBaseUrl}/realms/{keycloakRealm}",

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
    options.UseNpgsql(connectionString);
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

// To handle all the exceptions in the API
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();
