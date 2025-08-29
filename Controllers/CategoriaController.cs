using Catalogo.Models;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Microsoft.AspNetCore.Mvc;

namespace Catalogo.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CategoriaController : ControllerBase
{
    CategoriaRepository repository;
    public CategoriaController(CategoriaRepository _repository)
    {
        repository = _repository;
    }

    // Listar todas as categorias
    [HttpGet]
    public ActionResult<List<Categoria>> GetAll()
    {
        return repository.GetAll();
    }

    // Listar uma categoria pelo Id
    [HttpGet("{id}")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = repository.Get(id);
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

        repository.Add(categoria);
        return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
    }

    // Deletar uma categoria por id
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var categoria = repository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada.");

        repository.Delete(id);
        return NoContent();
    }

    // Atualizar uma categoria por id
    [HttpPut("{id}")]
    public IActionResult Update(int id, Categoria categoria)
    {
        if (id != categoria.Id)
            return BadRequest();

        var existingCategoria = repository.Get(id);
        if (existingCategoria is null)
            return NotFound("Categoria não encontrada.");

        repository.Update(categoria);
        return NoContent();
    }
}