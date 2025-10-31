using AuthService.Src.Configurations;
using AuthService.Src.Interfaces;
using AuthService.Src.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();