using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Catalogo.Models;
using Catalogo.Controllers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Catalogo.ObservabilityLab.Observability;

namespace Catalogo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("cadastro")]
    public async Task<IActionResult> Cadastro(CadastroDTO model)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("AuthController.Cadastro");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", model.Email ?? "unknown");

            _logger.LogInformation("Tentativa de cadastro para email: {Email}. TraceId: {TraceId}", model.Email, traceId);

            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Senha))
            {
                _logger.LogWarning("Tentativa de cadastro com email ou senha vazios. TraceId: {TraceId}", traceId);
                return BadRequest("Email e senha são obrigatórios.");
            }

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Senha);

            if (result.Succeeded)
            {
                _logger.LogInformation("Usuário cadastrado com sucesso: {Email}. TraceId: {TraceId}", model.Email, traceId);
                return Ok("Usuário registrado com sucesso!");
            }

            _logger.LogWarning("Falha no cadastro para {Email}: {Errors}. TraceId: {TraceId}", model.Email, string.Join(", ", result.Errors.Select(e => e.Description)), traceId);
            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro durante cadastro. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        try
        {
            var traceId = Activity.Current?.TraceId.ToString();
            using var activity = TelemetrySources.ActivitySource.StartActivity("AuthController.Login");

            activity?.SetTag("custom.traceId", traceId);
            activity?.SetTag("custom.user", model.Email ?? "unknown");

            _logger.LogInformation("Tentativa de login para email: {Email}. TraceId: {TraceId}", model.Email, traceId);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente: {Email}. TraceId: {TraceId}", model.Email, traceId);
                return Unauthorized("Usuário não encontrado.");
            }

            var validPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!validPassword)
            {
                _logger.LogWarning("Tentativa de login com senha inválida para: {Email}. TraceId: {TraceId}", model.Email, traceId);
                return Unauthorized("Senha inválida.");
            }

            _logger.LogInformation("Login bem-sucedido para: {Email}. TraceId: {TraceId}", model.Email, traceId);

            // Criar claims do token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // trocar por config segura
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "CatalogoApi",
                audience: "CatalogoApi",
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
        catch (Exception ex)
        {
            var traceId = Activity.Current?.TraceId.ToString();
            _logger.LogError(ex, "Erro durante login. TraceId: {TraceId}", traceId);
            return StatusCode(500, "Erro interno do servidor.");
        }
    }
}