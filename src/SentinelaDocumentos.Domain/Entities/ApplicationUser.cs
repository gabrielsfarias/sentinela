using Microsoft.AspNetCore.Identity;

namespace SentinelaDocumentos.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    // Seus campos personalizados aqui dentro (NomeEmpresa, CNPJ, etc.)
    public string? NomeEmpresa { get; set; }
    public string? CNPJ { get; set; }

    // Coleção de documentos (se você adicionou)
    public virtual ICollection<DocumentoEmpresa> Documentos { get; set; } = [];
}