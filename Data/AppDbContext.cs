using Microsoft.EntityFrameworkCore;
using Catalogo.Models;
using Catalogo.Data;

namespace Catalogo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Produto> Produtos { get; set; }
}