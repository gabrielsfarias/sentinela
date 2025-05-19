using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class Document
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? UserId { get; set; } // Chave estrangeira para o usuário

    [ForeignKey("UserId")]
    public virtual IdentityUser? User { get; set; } // Propriedade de navegação

    [Required]
    [MaxLength(260)] // Limite comum para nomes de arquivo
    public string? OriginalFileName { get; set; }

    [MaxLength(100)]
    public string? OriginalFileType { get; set; } // Mime type

    public long? OriginalFileSize { get; set; } // Em bytes

    public DateTime? OriginalFileLastModified { get; set; }

    [Required]
    [MaxLength(260)]
    public string? DisplayName { get; set; } // Nome editável, pode iniciar com OriginalFileName

    public DateTime? ExpiryDate { get; set; } // Data de validade, nullable

    [MaxLength(1000)]
    public string? Notes { get; set; } // Anotações adicionais

    // Timestamps automáticos
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Document()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
