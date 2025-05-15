using System.ComponentModel.DataAnnotations;
namespace Backend.Models;
public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}