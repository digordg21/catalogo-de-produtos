using Catalogo.Models;
using Catalogo.Data.Repository;
using Catalogo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Catalogo.ObservabilityLab.Observability;
using System.Linq.Expressions;

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
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.GetAll");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");

            _logger.LogInformation("Listando todas as categorias. TraceId: {TraceId}", traceId);

            var categorias = categoriaRepository.GetAll();
            _logger.LogInformation("Retornadas {Count} categorias. TraceId: {TraceId}", categorias.Count, traceId);
            activity?.SetTag("custom.categoria.count", categorias.Count);

            return categorias;

        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao listar categorias. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

        // Listar uma categoria pelo Id
        [Authorize]
        [HttpGet("{id}")]
        public ActionResult<Categoria> Get(int id)
        {
            try
            {
                var traceId = Activity.Current?.TraceId.ToString();
                using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.Get");

                activity?.SetTag("custom.traceId", traceId);
                activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
                activity?.SetTag("custom.categoria.id", id);

                _logger.LogInformation("Buscando categoria com ID {Id}. TraceId: {TraceId}", id, traceId);

                var categoria = categoriaRepository.Get(id);
                if (categoria is null)
                {
                    _logger.LogWarning("Categoria com ID {Id} não encontrada. TraceId: {TraceId}", id, traceId);
                    return NotFound("Categoria não encontrada.");
                }

                _logger.LogInformation("Categoria encontrada: {Nome}. TraceId: {TraceId}", categoria.Nome, traceId);
                return categoria;
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogError(ex, "Erro ao buscar categoria com ID {Id}. TraceId: {TraceId}", id, traceId);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // Criar uma nova categoria
        [Authorize]
        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            try
            {
                var traceId = Activity.Current?.TraceId.ToString();
                using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.Create");

                activity?.SetTag("custom.traceId", traceId);
                activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");

                _logger.LogInformation("Criando nova categoria: {Nome}. TraceId: {TraceId}", categoria.Nome, traceId);

                if (string.IsNullOrEmpty(categoria.Nome))
                {
                    _logger.LogWarning("Tentativa de criar categoria com nome vazio. TraceId: {TraceId}", traceId);
                    return BadRequest("O nome da categoria é obrigatório.");
                }

                categoriaRepository.Add(categoria);
                _logger.LogInformation("Categoria criada com ID {Id}. TraceId: {TraceId}", categoria.Id, traceId);
                activity?.SetTag("custom.categoria.id", categoria.Id);

                return CreatedAtAction(nameof(Get), new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogError(ex, "Erro ao criar categoria. TraceId: {TraceId}", traceId);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // Deletar uma categoria por id
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var traceId = Activity.Current?.TraceId.ToString();
                using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.Delete");

                activity?.SetTag("custom.traceId", traceId);
                activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
                activity?.SetTag("custom.categoria.id", id);

                _logger.LogInformation("Deletando categoria com ID {Id}. TraceId: {TraceId}", id, traceId);

                var categoria = categoriaRepository.Get(id);
                if (categoria is null)
                {
                    _logger.LogWarning("Tentativa de deletar categoria inexistente com ID {Id}. TraceId: {TraceId}", id, traceId);
                    return NotFound("Categoria não encontrada.");
                }

                categoriaRepository.Delete(id);
                _logger.LogInformation("Categoria deletada: {Nome}. TraceId: {TraceId}", categoria.Nome, traceId);

                return NoContent();
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogError(ex, "Erro ao deletar categoria com ID {Id}. TraceId: {TraceId}", id, traceId);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        // Simulador de erro 500
        [Authorize]
        [HttpGet("erro")]
        public IActionResult Error()
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.Error");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");

            _logger.LogInformation("Simulando erro 500 para teste de tracing. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor :)");
        }

        // Atualizar uma categoria por id
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, Categoria categoria)
        {
            try
            {
                var traceId = Activity.Current?.TraceId.ToString();
                using var activity = TelemetrySources.ActivitySource.StartActivity("CategoriaController.Update");

                activity?.SetTag("custom.traceId", traceId);
                activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
                activity?.SetTag("custom.categoria.id", id);

                _logger.LogInformation("Atualizando categoria com ID {Id} para {Nome}. TraceId: {TraceId}", id, categoria.Nome, traceId);

                if (id != categoria.Id)
                {
                    _logger.LogWarning("ID da URL ({UrlId}) não corresponde ao ID da categoria ({CategoriaId}). TraceId: {TraceId}", id, categoria.Id, traceId);
                    return BadRequest();
                }

                var existingCategoria = categoriaRepository.Get(id);
                if (existingCategoria is null)
                {
                    _logger.LogWarning("Tentativa de atualizar categoria inexistente com ID {Id}. TraceId: {TraceId}", id, traceId);
                    return NotFound("Categoria não encontrada.");
                }

                categoriaRepository.Update(categoria);
                _logger.LogInformation("Categoria atualizada: {Nome}. TraceId: {TraceId}", categoria.Nome, traceId);

                return NoContent();
            }
            catch (Exception ex)
            {
                var traceId = Activity.Current?.TraceId.ToString();
                _logger.LogError(ex, "Erro ao atualizar categoria com ID {Id}. TraceId: {TraceId}", id, traceId);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }