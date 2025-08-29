using Catalogo.Models;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Data.Repository;

public class CategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Categoria> GetAll()
    {
        return _context.Categorias.ToList();
    }

    public Categoria? Get(int id)
    {
        return _context.Categorias.AsNoTracking().FirstOrDefault(c => c.Id == id);
    }

    public void Add(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        _context.SaveChanges();
    }
    public void Update(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var categoria = Get(id);
        if (categoria is null)
            return;

        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
    }
}