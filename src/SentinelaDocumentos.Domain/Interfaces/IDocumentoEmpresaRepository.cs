using SentinelaDocumentos.Domain.Entities;

namespace SentinelaDocumentos.Domain.Interfaces;

public interface IDocumentoEmpresaRepository
{
    Task AdicionarAsync(DocumentoEmpresa documento);
    Task<DocumentoEmpresa?> ObterPorIdEUsuarioAsync(long id, string usuarioId); // Sempre no contexto do usuário
    Task<IEnumerable<DocumentoEmpresa>> ListarPorUsuarioAsync(string usuarioId); // Trazer todos do usuário
    Task<IEnumerable<DocumentoEmpresa>> ObterDocumentosProximosDoVencimentoAsync(DateTime dataLimite, int diasMinimos, string? usuarioId = null); // Buscar candidatos para alerta (pode ser para um usuário ou todos)
    Task AtualizarAsync(DocumentoEmpresa documento);
    Task DesativarAsync(DocumentoEmpresa documento); // Exclusão lógica
}