using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Application.DTOs.Auth
{
    public class LoginDto
    {
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}