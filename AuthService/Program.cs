using AuthService.Src.Configurations;
using AuthService.Src.Data;
using AuthService.Src.Interfaces;
using AuthService.Src.Services;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for microservices using gRPC and HTTP"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the text input below.\nExample: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configurar JWT Settings
var jwtSettings = new JwtSettings
{
    Secret = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("JWT_SECRET no configurado"),
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "CensudexAPIGateway",
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "CensudexClients",
    ExpirationMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "60")
};

builder.Services.AddSingleton(jwtSettings);

// Configurar DbContext para blacklist
var connectionString = $"Host={Environment.GetEnvironmentVariable("DATABASE_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("DATABASE_PORT")};" +
                      $"Database={Environment.GetEnvironmentVariable("DATABASE_NAME")};" +
                      $"Username={Environment.GetEnvironmentVariable("DATABASE_USER")};" +
                      $"Password={Environment.GetEnvironmentVariable("DATABASE_PASSWORD")}";

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    context.Database.Migrate();
}

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