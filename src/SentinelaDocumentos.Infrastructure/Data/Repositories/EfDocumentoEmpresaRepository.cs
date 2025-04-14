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

    // Extraindo lógica de filtro para um método auxiliar
    private IQueryable<DocumentoEmpresa> FiltrarDocumentosAtivos(string? usuarioId = null)
    {
        return context.DocumentosEmpresa.Where(d => d.Ativo && (usuarioId == null || d.ApplicationUserId == usuarioId));
    }

    public async Task<DocumentoEmpresa?> ObterPorIdEUsuarioAsync(long id, string usuarioId)
    {
        if (string.IsNullOrEmpty(usuarioId)) throw new ArgumentException("Usuário inválido.", nameof(usuarioId));

        return await FiltrarDocumentosAtivos(usuarioId)
            .Include(d => d.TipoDocumento)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ListarPorUsuarioAsync(string usuarioId)
    {
        return await FiltrarDocumentosAtivos(usuarioId)
            .Include(d => d.TipoDocumento)
            .OrderByDescending(d => d.DataValidade)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ObterDocumentosProximosDoVencimentoAsync(DateTime dataLimite, int diasMinimos, string? usuarioId = null)
    {
         // Busca documentos ativos cuja validade está entre hoje e a data limite
        // Inclui o Usuário para podermos pegar o Email para notificação
        return await FiltrarDocumentosAtivos(usuarioId)
            .Where(d => d.DataValidade >= DateTime.Now.AddDays(diasMinimos) && d.DataValidade <= dataLimite)
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