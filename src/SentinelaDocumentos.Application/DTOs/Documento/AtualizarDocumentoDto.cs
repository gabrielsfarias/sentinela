using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Application.DTOs.Documento
{
    public class AtualizarDocumentoDto : DocumentoBaseDto
    {
        [Required]
        public long Id { get; set; }
    }
}