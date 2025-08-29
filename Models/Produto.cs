using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Catalogo.Models;

public class Produto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string? Nome { get; set; }
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}