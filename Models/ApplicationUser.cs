using Microsoft.AspNetCore.Identity;

namespace Catalogo.Models;

public class ApplicationUser : IdentityUser
{
    // Propriedade personalizada para armazenar o nome completo do usuário
    public string FullName { get; set; } = string.Empty;

}