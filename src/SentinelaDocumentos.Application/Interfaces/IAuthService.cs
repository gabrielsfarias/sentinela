using SentinelaDocumentos.Application.DTOs.Auth;

namespace SentinelaDocumentos.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegistrarAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}