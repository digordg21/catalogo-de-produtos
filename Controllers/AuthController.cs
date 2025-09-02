using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Catalogo.Models;
using Catalogo.Controllers;

namespace Catalogo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastro(CadastroDTO model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Senha))
            return BadRequest("Email e senha são obrigatórios.");

        var user = new ApplicationUser
        {   
            FullName = model.FullName,
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Senha);

        if (result.Succeeded)
            return Ok("Usuário registrado com sucesso!");

        return BadRequest(result.Errors);
    }
}