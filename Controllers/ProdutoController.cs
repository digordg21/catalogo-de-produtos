using Microsoft.AspNetCore.Mvc;
using Catalogo.Data;
using Catalogo.Data.Repository;
using Catalogo.Models;

namespace Produtos.Controllers;

[ApiController]
[Route("api/[controller]")]

public class ProdutoController : ControllerBase
{
    ProdutoRepository produtoRepository;
    CategoriaRepository categoriaRepository;

    public ProdutoController(ProdutoRepository _produtoRepository, CategoriaRepository _categoriaRepository)
    {
        produtoRepository = _produtoRepository;
        categoriaRepository = _categoriaRepository;
    }

    // Listar todos os Produtos
    [HttpGet]
    public ActionResult<List<Produto>> GetAll()
    {
        return produtoRepository.GetAll();
    }

    // Listar produto por ID
    [HttpGet("{id}")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = produtoRepository.Get(id);
        if (produto is null)
            return NotFound("Produto não encontrado");

        return produto;
    }

    //Criar Produto
    [HttpPost]
    public ActionResult<Produto> Create(Produto produto)
    {
        // Validação de nome vazio
        if (string.IsNullOrWhiteSpace(produto.Nome))
            return BadRequest("Campo nome é obrigatório");
        // Validação de preço menor que zero
        if (produto.Preco < 0)
            return BadRequest("Preço não pode ser menor que R$ 0");

        // Validação de categoria existente
        var categoria = categoriaRepository.Get(produto.CategoriaId);
        if (categoria is null)
            return BadRequest("Categoria não encontrada");

        produtoRepository.Add(produto);
        return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
    }

    // Deletar um produto por ID
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var produto = produtoRepository.Get(id);
        if (produto is null)
            return NotFound("Produto Não Encontrado");

        produtoRepository.Delete(id);
        return NoContent();
    }

    // Atualizar um produto por Id
    [HttpPut("{id}")]
    public IActionResult Update(int id, Produto produto)
    {
        // Validação de nome vazio
        if (string.IsNullOrWhiteSpace(produto.Nome))
            return BadRequest("Campo nome é obrigatório");
        // Validação de preço menor que zero
        if (produto.Preco < 0)
            return BadRequest("Preço não pode ser menor que R$ 0");

        //Validação de produto existente
        var produtoExistente = produtoRepository.Get(id);
        if (produtoExistente is null)
            return NotFound("Produto não encontrado");


        // Validação de categoria existente
            var categoria = categoriaRepository.Get(produto.CategoriaId);
        if (categoria is null)
            return BadRequest("Categoria não encontrada");

        produtoRepository.Update(produto);
        return NoContent();
    }
}