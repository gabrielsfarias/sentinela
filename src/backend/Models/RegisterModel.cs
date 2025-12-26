using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    public string? Password { get; set; }
}
