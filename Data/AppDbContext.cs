using Microsoft.EntityFrameworkCore;
using Catalogo.Models;
using Catalogo.Data;

namespace Catalogo.Data;

public class AppDbContext : AppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas { get; set; }
}