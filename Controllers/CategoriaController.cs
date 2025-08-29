using Catalogo.Models;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Categorias.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CategoriaController : ControllerBase
{
    CategoriaRepository categoriaRepository;
    public CategoriaController(CategoriaRepository _repository)
    {
        categoriaRepository = _repository;
    }

    // Listar todas as categorias
    [HttpGet]
    public ActionResult<List<Categoria>> GetAll()
    {
        return categoriaRepository.GetAll();
    }

    // Listar uma categoria pelo Id
    [HttpGet("{id}")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = categoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada.");

        return categoria;
    }

    // Criar uma nova categoria
    [HttpPost]
    public IActionResult Create(Categoria categoria)
    {
        if (string.IsNullOrEmpty(categoria.Nome))
            return BadRequest("O nome da categoria é obrigatório.");

        categoriaRepository.Add(categoria);
        return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
    }

    // Deletar uma categoria por id
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var categoria = categoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada.");

        categoriaRepository.Delete(id);
        return NoContent();
    }

    // Atualizar uma categoria por id
    [HttpPut("{id}")]
    public IActionResult Update(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return BadRequest();

        var existingCategoria = categoriaRepository.Get(id);
        if (existingCategoria is null)
            return NotFound("Categoria não encontrada.");

        categoriaRepository.Update(categoria);
        return NoContent();
    }
}