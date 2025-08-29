using System.ComponentModel.DataAnnotations;
namespace Catalogo.Models;

public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    // Propriedade de navegação → uma categoria tem muitos produtos
    public ICollection<Produto>? Produtos { get; set; } = new List<Produto>();
}