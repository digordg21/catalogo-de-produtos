using Catalogo.Models;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Categorias.Controllers;

[ApiController]
[Route("api/[controller]")]

public class CategoriaController : ControllerBase
{
    CategoriaRepository categoriaRepository;
    private readonly ILogger<CategoriaController> _logger;
    public CategoriaController(CategoriaRepository _repository, ILogger<CategoriaController> logger)
    {
        categoriaRepository = _repository;
        _logger = logger;
    }

    // Listar todas as categorias
    [Authorize]
    [HttpGet]
    public ActionResult<List<Categoria>> GetAll()
    {
        var traceId = Activity.Current?.TraceId.ToString();

        _logger.LogInformation("Buscando todas as categorias. TraceId: {TraceId}", traceId);
        _logger.LogTrace("Buscando todas as categorias. TraceId: {TraceId}", traceId);
        
        return categoriaRepository.GetAll();
    }

    // Listar uma categoria pelo Id
    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Categoria> Get(int id)
    {
        var categoria = categoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada.");

        return categoria;
    }

    // Criar uma nova categoria
    [Authorize]
    [HttpPost]
    public IActionResult Create(Categoria categoria)
    {
        if (string.IsNullOrEmpty(categoria.Nome))
            return BadRequest("O nome da categoria é obrigatório.");

        categoriaRepository.Add(categoria);
        return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
    }

    // Deletar uma categoria por id
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var categoria = categoriaRepository.Get(id);
        if (categoria is null)
            return NotFound("Categoria não encontrada.");

        categoriaRepository.Delete(id);
        return NoContent();
    }

    // Simulador de erro 500
    [Authorize]
    [HttpGet("erro")]
    public IActionResult Error()
    {
        var traceId = Activity.Current?.TraceId.ToString();
        _logger.LogInformation("Simulando erro 500 para teste de tracing. TraceId: {TraceId}", traceId);
        return StatusCode(500, "Erro interno do servidor :)");
    }

    // Atualizar uma categoria por id
    [Authorize]
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