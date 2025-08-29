using Catalogo.Models;
using Catalogo.Data;

namespace Catalogo.Data.Repository;

public class ProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Categoria> GetAll()
    {
        return _context.Categorias.ToList();
    }
}