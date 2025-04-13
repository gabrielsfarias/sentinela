using Microsoft.EntityFrameworkCore;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Domain.Interfaces;

namespace SentinelaDocumentos.Infrastructure.Data.Repositories;

public class EfTipoDocumentoRepository(ApplicationDbContext context) : ITipoDocumentoRepository
{
    public async Task<IEnumerable<TipoDocumento>> ListarTodosAsync()
    {
        return await context.TiposDocumento.AsNoTracking().OrderBy(t => t.Nome).ToListAsync();
    }

    public async Task<TipoDocumento?> ObterPorIdAsync(int id)
    {
        return await context.TiposDocumento.FindAsync(id);
    }
}