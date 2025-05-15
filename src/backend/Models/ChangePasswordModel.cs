using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class ChangePasswordModel
{
    [Required(ErrorMessage = "Senha atual é obrigatória")]
    [DataType(DataType.Password)]
    public string? CurrentPassword { get; set; }

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "A {0} deve ter no mínimo {2} e no máximo {1} caracteres.", MinimumLength = 6)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "A nova senha e a senha de confirmação não conferem.")]
    public string? ConfirmNewPassword { get; set; }
}