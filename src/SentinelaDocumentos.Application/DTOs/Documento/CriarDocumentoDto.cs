using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Application.DTOs.Documento
{
    public abstract class DocumentoBaseDto
    {
        [StringLength(100)]
        public string? OrgaoEmissor { get; set; }

        public DateTime? DataEmissao { get; set; }

        [Required]
        public DateTime DataValidade { get; set; }

        [StringLength(500)]
        public string? Observacoes { get; set; }
    }

    public class CriarDocumentoDto : DocumentoBaseDto
    {
        [Required]
        public int TipoDocumentoId { get; set; }

        [StringLength(50, ErrorMessage = "O número do documento não pode exceder 50 caracteres.")]
        public string? NumeroDocumento { get; set; }
    }
}