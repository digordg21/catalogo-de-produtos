using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Catalogo.Data;
using Catalogo.Data.Repository;
using Catalogo.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Catalogo.ObservabilityLab.Observability;

namespace Produtos.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProdutoController : ControllerBase
{
    ProdutoRepository produtoRepository;
    CategoriaRepository categoriaRepository;
    private readonly ILogger<ProdutoController> _logger;

    public ProdutoController(ProdutoRepository _produtoRepository, CategoriaRepository _categoriaRepository, ILogger<ProdutoController> logger)
    {
        produtoRepository = _produtoRepository;
        categoriaRepository = _categoriaRepository;
        _logger = logger;
    }

    // Listar todos os Produtos
    [Authorize]
    [HttpGet]
    public ActionResult<List<Produto>> GetAll()
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.GetAll");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");

            _logger.LogInformation("Listando todos os produtos. TraceId: {TraceId}", traceId);

            var produtos = produtoRepository.GetAll();
            _logger.LogInformation("Retornados {Count} produtos. TraceId: {TraceId}", produtos.Count, traceId);
            activity?.SetTag("custom.produto.count", produtos.Count);

            return produtos;
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao listar produtos. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    // Listar produto por ID
    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<Produto> Get(int id)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.Get");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
            activity?.SetTag("custom.produto.id", id);

            _logger.LogInformation("Buscando produto com ID {Id}. TraceId: {TraceId}", id, traceId);

            var produto = produtoRepository.Get(id);
            if (produto is null)
            {
                _logger.LogWarning("Produto com ID {Id} não encontrado. TraceId: {TraceId}", id, traceId);
                return NotFound("Produto não encontrado");
            }

            _logger.LogInformation("Produto encontrado: {Nome}. TraceId: {TraceId}", produto.Nome, traceId);
            return produto;
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao buscar produto com ID {Id}. TraceId: {TraceId}", id, traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    //Criar Produto
    [Authorize]
    [HttpPost]
    public ActionResult<Produto> Create(Produto produto)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.Create");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");

            _logger.LogInformation("Criando novo produto: {Nome}. TraceId: {TraceId}", produto.Nome, traceId);

            // Validação de nome vazio
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                _logger.LogWarning("Tentativa de criar produto com nome vazio. TraceId: {TraceId}", traceId);
                return BadRequest("Campo nome é obrigatório");
            }
            // Validação de preço menor que zero
            if (produto.Preco < 0)
            {
                _logger.LogWarning("Tentativa de criar produto com preço negativo: {Preco}. TraceId: {TraceId}", produto.Preco, traceId);
                return BadRequest("Preço não pode ser menor que R$ 0");
            }

            // Validação de categoria existente
            var categoria = categoriaRepository.Get(produto.CategoriaId);
            if (categoria is null)
            {
                _logger.LogWarning("Tentativa de criar produto com categoria inexistente ID {CategoriaId}. TraceId: {TraceId}", produto.CategoriaId, traceId);
                return BadRequest("Categoria não encontrada");
            }

            produtoRepository.Add(produto);
            _logger.LogInformation("Produto criado com ID {Id}. TraceId: {TraceId}", produto.Id, traceId);
            activity?.SetTag("custom.produto.id", produto.Id);

            return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao criar produto. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    // Deletar um produto por ID
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.Delete");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
            activity?.SetTag("custom.produto.id", id);

            _logger.LogInformation("Deletando produto com ID {Id}. TraceId: {TraceId}", id, traceId);

            var produto = produtoRepository.Get(id);
            if (produto is null)
            {
                _logger.LogWarning("Tentativa de deletar produto inexistente com ID {Id}. TraceId: {TraceId}", id, traceId);
                return NotFound("Produto Não Encontrado");
            }

            produtoRepository.Delete(id);
            _logger.LogInformation("Produto deletado: {Nome}. TraceId: {TraceId}", produto.Nome, traceId);

            return NoContent();
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao deletar produto com ID {Id}. TraceId: {TraceId}", id, traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    // Atualizar um produto por Id
    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id, Produto produto)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.Update");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
            activity?.SetTag("custom.produto.id", id);

            _logger.LogInformation("Atualizando produto com ID {Id} para {Nome}. TraceId: {TraceId}", id, produto.Nome, traceId);

            // Validação de nome vazio
            if (string.IsNullOrWhiteSpace(produto.Nome))
            {
                _logger.LogWarning("Tentativa de atualizar produto com nome vazio. TraceId: {TraceId}", traceId);
                return BadRequest("Campo nome é obrigatório");
            }
            // Validação de preço menor que zero
            if (produto.Preco < 0)
            {
                _logger.LogWarning("Tentativa de atualizar produto com preço negativo: {Preco}. TraceId: {TraceId}", produto.Preco, traceId);
                return BadRequest("Preço não pode ser menor que R$ 0");
            }

            //Validação de produto existente
            var produtoExistente = produtoRepository.Get(id);
            if (produtoExistente is null)
            {
                _logger.LogWarning("Tentativa de atualizar produto inexistente com ID {Id}. TraceId: {TraceId}", id, traceId);
                return NotFound("Produto não encontrado");
            }

            // Validação de categoria existente
            var categoria = categoriaRepository.Get(produto.CategoriaId);
            if (categoria is null)
            {
                _logger.LogWarning("Tentativa de atualizar produto com categoria inexistente ID {CategoriaId}. TraceId: {TraceId}", produto.CategoriaId, traceId);
                return BadRequest("Categoria não encontrada");
            }

            produtoRepository.Update(produto);
            _logger.LogInformation("Produto atualizado: {Nome}. TraceId: {TraceId}", produto.Nome, traceId);

            return NoContent();
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao atualizar produto com ID {Id}. TraceId: {TraceId}", id, traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    [Authorize]
    [HttpGet("paged")]
    public ActionResult<PagedResult<Produto>> GetAllPaged(
        int? categoriaId,
        decimal? minPrice,
        decimal? maxPrice,
        int page = 1,
        int pageSize = 5
    )
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("ProdutoController.GetAllPaged");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", User.Identity?.Name ?? "anonymous");
            activity?.SetTag("custom.page", page);
            activity?.SetTag("custom.pageSize", pageSize);

            _logger.LogInformation("Listando produtos paginados: página {Page}, tamanho {PageSize}. TraceId: {TraceId}", page, pageSize, traceId);

            var result = produtoRepository.GetAllPaged(categoriaId, minPrice, maxPrice, page, pageSize);
            _logger.LogInformation("Retornados {Count} produtos na página {Page}. TraceId: {TraceId}", result.Items.Count, page, traceId);
            activity?.SetTag("custom.produto.count", result.Items.Count);
            activity?.SetTag("custom.totalCount", result.TotalCount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro ao listar produtos paginados. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

}