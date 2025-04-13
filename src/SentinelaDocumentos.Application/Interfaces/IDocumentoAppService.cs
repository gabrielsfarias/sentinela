using System.Collections.Generic;
using System.Threading.Tasks;
using SentinelaDocumentos.Application.DTOs.Documento;

namespace SentinelaDocumentos.Application.Interfaces
{
    public interface IDocumentoAppService
    {
        Task<DocumentoDto> AdicionarDocumentoAsync(CriarDocumentoDto dto, string usuarioId);
        Task<IEnumerable<DocumentoDto>> ListarDocumentosAsync(string usuarioId, int page, int pageSize);
        Task<DocumentoDto> ObterDetalhesDocumentoAsync(long id, string usuarioId);
        Task AtualizarDocumentoAsync(AtualizarDocumentoDto dto, string usuarioId);
        Task DesativarDocumentoAsync(long id, string usuarioId);
    }
}