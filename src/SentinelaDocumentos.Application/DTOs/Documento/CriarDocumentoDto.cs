using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Application.DTOs.Documento
{
    public abstract class DocumentoBaseDto
    {
        [StringLength(100)]
        public string? OrgaoEmissor { get; set; }

        [StringLength(50)]
        public string? NumeroDocumento { get; set; }

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
    }
}