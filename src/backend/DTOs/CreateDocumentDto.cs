using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

public class CreateDocumentDto
{
    [Required(ErrorMessage = "Nome original do arquivo é obrigatório.")]
    [MaxLength(260)]
    public string? OriginalFileName { get; set; }

    [MaxLength(100)]
    public string? OriginalFileType { get; set; }

    public long? OriginalFileSize { get; set; }
    public DateTime? OriginalFileLastModified { get; set; }

    [MaxLength(260)]
    public string? DisplayName { get; set; } // Opcional na criação, pode ser preenchido com OriginalFileName no backend

    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}
