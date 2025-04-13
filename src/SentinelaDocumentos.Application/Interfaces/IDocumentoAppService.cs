using SentinelaDocumentos.Application.DTOs.Documento;

namespace SentinelaDocumentos.Application.Interfaces
{
    public interface IDocumentoAppService
    {
        Task<IEnumerable<DocumentoDto>> AdicionarDocumentoAsync(CriarDocumentoDto dto, string usuarioId);
        Task<IEnumerable<DocumentoDto>> ListarDocumentosAsync(string usuarioId, int page, int pageSize);
        Task<IEnumerable<DocumentoDto>> ObterDetalhesDocumentoAsync(long id, string usuarioId);
        Task<IEnumerable<DocumentoDto>> AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId);
        Task<IEnumerable<DocumentoDto>> DesativarDocumentoAsync(long id, string usuarioId);
    }
}