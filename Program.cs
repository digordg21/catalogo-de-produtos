 using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Catalogo.Models;

var builder = WebApplication.CreateBuilder(args);


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

var app = builder.Build();


// Configurar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // 👈 Habilita Controllers

app.Run();
