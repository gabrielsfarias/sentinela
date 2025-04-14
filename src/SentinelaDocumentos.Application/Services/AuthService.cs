using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SentinelaDocumentos.Application.DTOs.Auth;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Application.DTOs.User;

namespace SentinelaDocumentos.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger) : IAuthService
    {
        public async Task<AuthResponseDto> RegistrarAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                logger.LogWarning("Tentativa de registro com senhas não coincidentes para o email {Email}", dto.Email);
                return new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "As senhas não coincidem.", 
                    Token = string.Empty, 
                    UserInfo = null 
                };
            }

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                logger.LogWarning("Tentativa de registro com email já existente: {Email}", dto.Email);
                return new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "O email já está em uso.", 
                    Token = string.Empty, 
                    UserInfo = null 
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                NomeEmpresa = dto.NomeEmpresa
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "Registro realizado com sucesso.", 
                    Token = "GeneratedTokenHere", // Substituir pela lógica real de geração de token
                    UserInfo = new UserInfoDto 
                    { 
                        Email = user.Email, 
                        UserName = user.UserName 
                    } 
                };
            }

            return new AuthResponseDto 
            { 
                IsSuccess = false, 
                Message = string.Join(", ", result.Errors.Select(e => e.Description)), 
                Token = string.Empty, 
                UserInfo = null 
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var result = await signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "Login realizado com sucesso.", 
                    Token = "GeneratedTokenHere", // Substituir pela lógica real de geração de token
                    UserInfo = new UserInfoDto 
                    { 
                        Email = dto.Email, 
                        UserName = dto.Email 
                    } 
                };
            }

            return new AuthResponseDto 
            { 
                IsSuccess = false, 
                Message = "Credenciais inválidas.", 
                Token = string.Empty, 
                UserInfo = null 
            };
        }
    }
}