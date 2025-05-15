using System.ComponentModel.DataAnnotations;
namespace Backend.Models;
public class ResetPasswordModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Token { get; set; } // Token virá da URL, mas será postado

    [Required]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "As senhas não conferem.")]
    public string? ConfirmNewPassword { get; set; }
}