using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

// Para PATCH, todos os campos são opcionais, pois o cliente envia apenas o que mudou.
// Validações como [Required] não fazem muito sentido aqui, a menos que um campo,
// uma vez definido, não possa ser tornado nulo.
public class UpdateDocumentDto
{
    [MaxLength(260)]
    public string? DisplayName { get; set; }

    public DateTime? ExpiryDate { get; set; } // Permitir definir como nulo para remover a data

    [MaxLength(1000)]
    public string? Notes { get; set; } // Permitir definir como nulo para limpar as notas
}
