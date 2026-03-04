using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Catalogo.Models;
using Catalogo.ObservabilityLab.Observability;
using Serilog;
using Serilog.Events;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

// nova configuração do OpenTelemetry, extraída para classe separada para deixar o Program.cs mais limpo
builder.Services.AddCustomOpenTelemetry();

// Adicionar suporte a controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configurar EF Core com SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=catalogo.db"));

builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<ProdutoRepository>();

// adicionando cfg para tokens JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "chave-super-secreta"; // depois coloca no appsettings.json
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CatalogoApi";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Configurar Serilog -- antigo
// Log.Logger = new LoggerConfiguration()
//     .MinimumLevel.Information()
//     .Enrich.FromLogContext()
//     .Enrich.WithMachineName()
//     .Enrich.WithThreadId()
//     .Enrich.WithSpan() // <- pega TraceId automaticamente
//     .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
//     .WriteTo.File(
//     new Serilog.Formatting.Json.JsonFormatter(),
//     "logs/log-.json",
//     rollingInterval: RollingInterval.Day)
//     .CreateLogger();

// builder.Host.UseSerilog();

// nova configuração do Serilog, extraída para classe separada para deixar o Program.cs mais limpo
builder.Host.UseCustomSerilog();

var app = builder.Build();

// Endpoint para métricas do Prometheus
app.MapPrometheusScrapingEndpoint();

// adicionando serilog request logging
app.UseSerilogRequestLogging(); // <- isso já loga todas as requisições com TraceId, status code, etc.

// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // 👈 Habilita Controllers


app.Run();
