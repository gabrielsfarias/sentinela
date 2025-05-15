using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger; // Adicionado para logging

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IConfiguration configuration,
        ILogger<AuthController> logger) // Adicionado para logging
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _logger = logger; // Adicionado para logging
    }

    [HttpPost("/cadastro")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuário {Email} cadastrado com sucesso.", model.Email);
            return Ok(new { Message = "Cadastro realizado com sucesso! Você pode fazer o login." });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        _logger.LogWarning("Falha ao cadastrar usuário {Email}: {Errors}", model.Email, result.Errors.Select(e => e.Description));
        return BadRequest(ModelState);
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            _logger.LogWarning("Tentativa de login falhou para email {Email}: usuário não encontrado.", model.Email);
            return Unauthorized(new { Message = "Email ou senha inválidos." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            // Adicionar roles se você as usar no futuro
            // var userRoles = await _userManager.GetRolesAsync(user);
            // foreach (var userRole in userRoles)
            // {
            //     authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            // }

            var jwtSecret = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32) // Boa prática verificar o tamanho
            {
                _logger.LogError("JWT Secret não está configurada corretamente ou é muito curta.");
                throw new InvalidOperationException("Configuração de segurança do servidor incompleta.");
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddHours(3), // Usar UtcNow para expiração
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Usuário {Email} logado com sucesso.", model.Email);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                expiration = token.ValidTo,
                email = user.Email
            });
        }
        _logger.LogWarning("Tentativa de login falhou para email {Email}: senha inválida.", model.Email);
        return Unauthorized(new { Message = "Email ou senha inválidos." });
    }

    [HttpPost("/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            // Não revele que o usuário não existe.
            _logger.LogInformation("Solicitação de reset de senha para email (não encontrado ou não revelado) {Email}.", model.Email);
            return Ok(new { Message = "Se o seu email estiver cadastrado em nosso sistema, você receberá um link para redefinir sua senha." });
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Obter a URL base do frontend das configurações ou definir um padrão para desenvolvimento
        var frontendBaseUrl = _configuration["FrontendApp:BaseUrl"] ?? "http://localhost:5173"; // Porta padrão do Vite
        var resetLink = $"{frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

        // TODO: Implementar lógica real de envio de email com o 'resetLink'
        // Exemplo: await _emailSender.SendPasswordResetLinkAsync(user.Email, resetLink);

        _logger.LogInformation("Link de reset de senha gerado para {Email}. Link (DEV ONLY): {Link}", user.Email, resetLink);

        // Em produção, você não retornaria o link ou o token.
        // A mensagem deve ser sempre genérica.
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return Ok(new {
                Message = "Se o seu email estiver cadastrado, você receberá um link. (Link de desenvolvimento abaixo)",
                DevelopmentOnlyLink = resetLink
            });
        }
        return Ok(new { Message = "Se o seu email estiver cadastrado em nosso sistema, você receberá um link para redefinir sua senha." });
    }

    [HttpPost("/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            // Não revele que o usuário não existe.
            _logger.LogWarning("Tentativa de reset de senha falhou para email (não encontrado ou não revelado) {Email}.", model.Email);
            return BadRequest(new { Message = "Não foi possível redefinir a senha. O link pode ser inválido ou ter expirado." });
        }

        // O token precisa ser decodificado se foi modificado (ex: espaços viraram '+')
        // Uri.UnescapeDataString(model.Token!) pode ser necessário se o token tiver caracteres especiais
        // que foram codificados na URL e depois decodificados incorretamente pelo model binding.
        // Normalmente, o model binding lida com isso, mas é um ponto a observar.
        var result = await _userManager.ResetPasswordAsync(user, model.Token!, model.NewPassword!);

        if (result.Succeeded)
        {
            _logger.LogInformation("Senha resetada com sucesso para o usuário {Email}.", model.Email);
            return Ok(new { Message = "Sua senha foi redefinida com sucesso. Você já pode fazer o login com a nova senha." });
        }

        var errors = result.Errors.Select(e => e.Description).ToList();
        _logger.LogWarning("Falha ao resetar senha para {Email}: {Errors}", model.Email, errors);
        // Não retorne todos os erros detalhados do Identity para o cliente por segurança, a menos que sejam genéricos.
        return BadRequest(new { Message = "Não foi possível redefinir a senha. Verifique se a nova senha atende aos critérios ou tente novamente.", Errors = errors });
    }
}