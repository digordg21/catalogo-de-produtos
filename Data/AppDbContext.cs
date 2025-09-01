using Microsoft.EntityFrameworkCore;
using Catalogo.Models;
using Catalogo.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Catalogo.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias { get; set; }

    public DbSet<Produto> Produtos { get; set; }
}