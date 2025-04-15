using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SentinelaDocumentos.Application.DTOs.Auth;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Application.DTOs.User;

namespace SentinelaDocumentos.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger, JwtService jwtService) : IAuthService
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

            var usuarioExistente = await userManager.FindByEmailAsync(dto.Email);
            if (usuarioExistente != null)
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
                var token = jwtService.GerarToken(user);
                return new AuthResponseDto 
                { 
                    IsSuccess = true, 
                    Message = "Registro realizado com sucesso.", 
                    Token = token,
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
                    var user = await userManager.FindByEmailAsync(dto.Email);
                    if (user != null)
                    {
                        var token = jwtService.GerarToken(user);
                        return new AuthResponseDto 
                        { 
                            IsSuccess = true, 
                            Message = "Login realizado com sucesso.", 
                            Token = token, 
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
                        Message = "Usuário não encontrado.", 
                        Token = string.Empty, 
                        UserInfo = null 
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