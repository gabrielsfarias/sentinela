using SentinelaDocumentos.Application.DTOs.Documento;

namespace SentinelaDocumentos.Application.Services.Utils
{
    public static class DocumentoUtils
    {
        public static void CalcularDetalhes(DocumentoDto dto)
        {
            dto.DiasParaVencer = (dto.DataValidade - DateTime.UtcNow).Days;
            dto.Status = CalcularStatusDocumento(dto.DataValidade);
        }

        public static string CalcularStatusDocumento(DateTime dataValidade)
        {
            if (dataValidade < DateTime.UtcNow)
                return "Vencido";
            if ((dataValidade - DateTime.UtcNow).Days <= 30)
                return "Próximo Vencimento";
            return "Válido";
        }
    }
}