namespace SentinelaDocumentos.Application.DTOs.User;

public class UserInfoDto
{
// Propriedades que você quer retornar sobre o usuário logado/registrado
    public string? Id { get; set; } // O Id do IdentityUser é string
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
