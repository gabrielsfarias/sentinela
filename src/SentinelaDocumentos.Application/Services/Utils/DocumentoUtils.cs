using System;

namespace SentinelaDocumentos.Application.Services.Utils
{
    public static class DocumentoUtils
    {
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