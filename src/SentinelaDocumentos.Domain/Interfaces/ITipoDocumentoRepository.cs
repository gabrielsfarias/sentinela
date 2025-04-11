using SentinelaDocumentos.Domain.Entities;

namespace SentinelaDocumentos.Domain.Interfaces;

public interface ITipoDocumentoRepository
{
    Task<IEnumerable<TipoDocumento>> ListarTodosAsync();
    Task<TipoDocumento?> ObterPorIdAsync(int id);
    // Talvez AddAsync se permitir customização futura
}