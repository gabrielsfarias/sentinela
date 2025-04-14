using SentinelaDocumentos.Application.DTOs.Documento;

namespace SentinelaDocumentos.Application.Services.Utils
{
    public static class DocumentoUtils
    {
        public static void CalcularDetalhes(DocumentoDto dto)
        {
            dto.DiasParaVencer = CalcularDiasParaVencer(dto.DataValidade);
            dto.Status = CalcularStatusDocumento(dto.DataValidade);
        }

        public static int CalcularDiasParaVencer(DateTime dataValidade)
        {
            return (dataValidade - DateTime.UtcNow).Days;
        }

        public static string CalcularStatusDocumento(DateTime dataValidade)
        {
            if (dataValidade < DateTime.UtcNow)
                return "Vencido";
            if (CalcularDiasParaVencer(dataValidade) <= 30)
                return "Próximo Vencimento";
            return "Válido";
        }
    }
}