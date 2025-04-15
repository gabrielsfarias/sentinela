using System.Text.Json.Serialization;

namespace SentinelaDocumentos.Application.DTOs.Documento
{
    public class AtualizarDocumentoDto : DocumentoBaseDto
    {
        [JsonIgnore]
        public long Id { get; set; }
        public int TipoDocumentoId { get; set; }
    }
}