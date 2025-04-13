using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Application.DTOs.Auth
{
    public class RegisterDto
    {
       
        [EmailAddress]
        public required string Email { get; set; }

    
        [DataType(DataType.Password)]
        public required string Password { get; set; }

 
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        public required string NomeEmpresa { get; set; }
    }
}