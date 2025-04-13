using Microsoft.EntityFrameworkCore;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Domain.Interfaces;

namespace SentinelaDocumentos.Infrastructure.Data.Repositories;

public class EfDocumentoEmpresaRepository(ApplicationDbContext context) : IDocumentoEmpresaRepository
{
    public async Task AdicionarAsync(DocumentoEmpresa documento)
    {
        ArgumentNullException.ThrowIfNull(documento);

        // Garante valores padrão antes de salvar
        documento.DataRegistroUtc = DateTime.UtcNow;
        documento.Ativo = true;
        await context.DocumentosEmpresa.AddAsync(documento);
        await context.SaveChangesAsync();
    }

    public async Task<DocumentoEmpresa?> ObterPorIdEUsuarioAsync(long id, string usuarioId)
    {
        if (string.IsNullOrEmpty(usuarioId)) throw new ArgumentException("Usuário inválido.", nameof(usuarioId));

        // Busca pelo Id, garantindo que pertence ao usuário e está ativo
        // Include busca o TipoDocumento relacionado (Eager Loading)
        // AsNoTracking para otimizar leitura
        return await context.DocumentosEmpresa.Include(d => d.TipoDocumento)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.Id == id && d.ApplicationUserId == usuarioId && d.Ativo);
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ListarPorUsuarioAsync(string usuarioId)
    {
        return await context.DocumentosEmpresa.Where(d => d.ApplicationUserId == usuarioId && d.Ativo)
                       .Include(d => d.TipoDocumento) // Traz o nome do tipo junto
                       .OrderByDescending(d => d.DataValidade) // Ordena pelos mais próximos de vencer primeiro
                       .AsNoTracking()
                       .ToListAsync();
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ObterDocumentosProximosDoVencimentoAsync(DateTime dataLimite, int diasMinimos, string? usuarioId = null)
    {
         // Busca documentos ativos cuja validade está entre hoje e a data limite
        // Inclui o Usuário para podermos pegar o Email para notificação
        return await context.DocumentosEmpresa
            .Where(d => d.Ativo && d.DataValidade >= DateTime.Now.AddDays(diasMinimos) && d.DataValidade <= dataLimite && (usuarioId == null || d.ApplicationUserId == usuarioId))
            .Include(d => d.Usuario)
            .OrderBy(d => d.DataValidade)
            .ToListAsync();
    }

    public async Task AtualizarAsync(DocumentoEmpresa documento)
    {
        ArgumentNullException.ThrowIfNull(documento);
        
        context.Entry(documento).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DesativarAsync(DocumentoEmpresa documento)
    {
        documento.Ativo = false;
        context.Entry(documento).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}