namespace SentinelaDocumentos.Application.DTOs.Documento
{
    public class DocumentoDto
    {
        public long Id { get; set; }
        public string NomeTipoDocumento { get; set; } = string.Empty;
        public string? OrgaoEmissor { get; set; }
        public string? NumeroDocumento { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime DataValidade { get; set; }
        public string? Observacoes { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? DiasParaVencer { get; set; }
    }
}