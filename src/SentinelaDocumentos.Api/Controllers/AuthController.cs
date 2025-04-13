using Microsoft.AspNetCore.Mvc;
using SentinelaDocumentos.Application.DTOs.Auth;
using SentinelaDocumentos.Application.Interfaces;

namespace SentinelaDocumentos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.RegistrarAsync(dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await authService.LoginAsync(dto);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }
    }
}