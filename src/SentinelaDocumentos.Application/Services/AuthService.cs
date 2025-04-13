using Microsoft.AspNetCore.Identity;
using SentinelaDocumentos.Application.DTOs.Auth;
using SentinelaDocumentos.Application.Interfaces;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Application.DTOs.User;

namespace SentinelaDocumentos.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : IAuthService
    {
        private const string PasswordMismatchMessage = "Passwords do not match.";
        private const string EmailExistsMessage = "Email already exists.";
        private const string RegistrationSuccessMessage = "User registered successfully.";
        private const string LoginSuccessMessage = "Login successful.";
        private const string InvalidLoginMessage = "Invalid login attempt.";

        public async Task<AuthResponseDto> RegistrarAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = PasswordMismatchMessage, 
                    Token = string.Empty, 
                    UserInfo = null 
                };
            }

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new AuthResponseDto 
                { 
                    IsSuccess = false, 
                    Message = EmailExistsMessage, 
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
                    Message = RegistrationSuccessMessage, 
                    Token = "GeneratedTokenHere", // Replace with actual token generation logic
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
                Token = string.Empty, // Provide a default or meaningful value
                UserInfo = null // Provide a default or meaningful value
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
                    Message = LoginSuccessMessage, 
                    Token = "GeneratedTokenHere", // Replace with actual token generation logic
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
                Message = InvalidLoginMessage, 
                Token = string.Empty, // Provide a default or meaningful value
                UserInfo = null // Provide a default or meaningful value
            };
        }
    }
}