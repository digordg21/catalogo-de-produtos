using Catalogo.Models;
using Catalogo.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalogo.Data.Repository;

public class ProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Produto> GetAll()
    {
        return _context.Produtos.ToList();
    }

    public Produto? Get(int id)
    {
        return _context.Produtos.AsNoTracking().FirstOrDefault(p => p.Id == id);
    }

    public void Add(Produto produto)
    {
        _context.Produtos.Add(produto);
        _context.SaveChanges();
    }
    public void Update(Produto produto)
    {
        _context.Produtos.Update(produto);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var produto = Get(id);
        if (produto is null)
            return;

        _context.Produtos.Remove(produto);
        _context.SaveChanges();
    }
}