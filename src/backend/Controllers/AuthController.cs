using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers;

[ApiController]
public class AuthController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IConfiguration configuration,
    ILogger<AuthController> logger
) : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly SignInManager<IdentityUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<AuthController> _logger = logger;

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
        _logger.LogWarning(
            "Falha ao cadastrar usuário {Email}: {Errors}",
            model.Email,
            result.Errors.Select(e => e.Description)
        );
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
            _logger.LogWarning(
                "Tentativa de login falhou para email {Email}: usuário não encontrado.",
                model.Email
            );
            return Unauthorized(new { Message = "Email ou senha inválidos." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            model.Password!,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var jwtSecret = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 32)
            {
                _logger.LogError("JWT Secret não está configurada corretamente ou é muito curta.");
                throw new InvalidOperationException(
                    "Configuração de segurança do servidor incompleta."
                );
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = _configuration["JWT:ValidIssuer"],
                Audience = _configuration["JWT:ValidAudience"],
                SigningCredentials = new SigningCredentials(
                    authSigningKey,
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Usuário {Email} logado com sucesso.", model.Email);
            return Ok(
                new
                {
                    token = tokenHandler.WriteToken(token),
                    expiration = token.ValidTo,
                    email = user.Email,
                }
            );
        }
        _logger.LogWarning(
            "Tentativa de login falhou para email {Email}: senha inválida.",
            model.Email
        );
        return Unauthorized(new { Message = "Email ou senha inválidos." });
    }

    [HttpPost("/forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            _logger.LogInformation(
                "Solicitação de reset de senha para email (não encontrado ou não revelado) {Email}.",
                model.Email
            );
            return Ok(
                new
                {
                    Message = "Se o seu email estiver cadastrado em nosso sistema, você receberá um link para redefinir sua senha.",
                }
            );
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var frontendBaseUrl = _configuration["FrontendApp:BaseUrl"] ?? "http://localhost:5173";
        var resetLink =
            $"{frontendBaseUrl}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";

        _logger.LogInformation(
            "Link de reset de senha gerado para {Email}. Link (DEV ONLY): {Link}",
            user.Email,
            resetLink
        );

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            return Ok(
                new
                {
                    Message = "Se o seu email estiver cadastrado, você receberá um link. (Link de desenvolvimento abaixo)",
                    DevelopmentOnlyLink = resetLink,
                }
            );
        }
        return Ok(
            new
            {
                Message = "Se o seu email estiver cadastrado em nosso sistema, você receberá um link para redefinir sua senha.",
            }
        );
    }

    [HttpPost("/reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            _logger.LogWarning(
                "Tentativa de reset de senha falhou para email (não encontrado ou não revelado) {Email}.",
                model.Email
            );
            return BadRequest(
                new
                {
                    Message = "Não foi possível redefinir a senha. O link pode ser inválido ou ter expirado.",
                }
            );
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Token!, model.NewPassword!);

        if (result.Succeeded)
        {
            _logger.LogInformation(
                "Senha resetada com sucesso para o usuário {Email}.",
                model.Email
            );
            return Ok(
                new
                {
                    Message = "Sua senha foi redefinida com sucesso. Você já pode fazer o login com a nova senha.",
                }
            );
        }

        var errors = result.Errors.Select(e => e.Description).ToList();
        _logger.LogWarning("Falha ao resetar senha para {Email}: {Errors}", model.Email, errors);
        return BadRequest(
            new
            {
                Message = "Não foi possível redefinir a senha. Verifique se a nova senha atende aos critérios ou tente novamente.",
                Errors = errors,
            }
        );
    }

    [Authorize]
    [HttpPost("/change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Obter o usuário logado (pelo token JWT)
        // O ClaimTypes.NameIdentifier contém o ID do usuário que foi colocado no token durante o login.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning(
                "ChangePassword: Tentativa de mudança de senha sem UserID no token."
            );
            return Unauthorized(new { Message = "Usuário não autenticado corretamente." });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning(
                "ChangePassword: Usuário com ID {UserId} não encontrado, mas token era válido.",
                userId
            );
            return NotFound(new { Message = "Usuário não encontrado." }); // Ou Unauthorized
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(
            user,
            model.CurrentPassword!,
            model.NewPassword!
        );

        if (changePasswordResult.Succeeded)
        {
            _logger.LogInformation(
                "Usuário {Email} (ID: {UserId}) alterou a senha com sucesso.",
                user.Email,
                userId
            );
            return Ok(new { Message = "Senha alterada com sucesso." });
        }

        var errors = changePasswordResult.Errors.Select(e => e.Description).ToList();
        _logger.LogWarning(
            "Falha ao alterar senha para usuário {Email} (ID: {UserId}): {Errors}",
            user.Email,
            userId,
            errors
        );
        // Não retorne todos os erros detalhados do Identity para o cliente por segurança, a menos que sejam genéricos.
        return BadRequest(
            new
            {
                Message = "Não foi possível alterar a senha. Verifique sua senha atual e se a nova senha atende aos critérios.",
                Errors = errors,
            }
        );
    }
}
