using Catalogo.Models;
using Catalogo.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

    public PagedResult<Produto> GetAllPaged(
        int? categoriaId,
        decimal? minPrice,
        decimal? maxPrice,
        int page = 1,
        int pageSize = 5
    )
    {
        var query = _context.Produtos.AsQueryable();

        // Filtros
        if (categoriaId.HasValue)
            query = query.Where(p => p.CategoriaId == categoriaId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Preco >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Preco <= maxPrice.Value);

        // Total de itens antes da paginação
        var totalItems = query.Count();

        // Paginação
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<Produto>
        {
            Items = items,
            TotalCount = totalItems,
            CurrentPage = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}