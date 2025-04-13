using Microsoft.EntityFrameworkCore;
using SentinelaDocumentos.Domain.Entities;
using SentinelaDocumentos.Domain.Interfaces;

namespace SentinelaDocumentos.Infrastructure.Data.Repositories;

public class EfDocumentoEmpresaRepository(ApplicationDbContext context) : IDocumentoEmpresaRepository
{
    public async Task AdicionarAsync(DocumentoEmpresa documento)
    {
        await context.DocumentosEmpresa.AddAsync(documento);
        await context.SaveChangesAsync();
    }

    public async Task<DocumentoEmpresa?> ObterPorIdEUsuarioAsync(long id, string usuarioId)
    {
        return await context.DocumentosEmpresa.FirstOrDefaultAsync(d => d.Id == id && d.ApplicationUserId == usuarioId && d.Ativo);
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ListarPorUsuarioAsync(string usuarioId)
    {
        return await context.DocumentosEmpresa.Where(d => d.ApplicationUserId == usuarioId && d.Ativo).ToListAsync();
    }

    public async Task<IEnumerable<DocumentoEmpresa>> ObterDocumentosProximosDoVencimentoAsync(DateTime dataLimite, int diasMinimos, string? usuarioId = null)
    {
        return await context.DocumentosEmpresa
            .Where(d => d.Ativo && d.DataValidade >= DateTime.Now.AddDays(diasMinimos) && d.DataValidade <= dataLimite && (usuarioId == null || d.ApplicationUserId == usuarioId))
            .OrderBy(d => d.DataValidade)
            .ToListAsync();
    }

    public async Task AtualizarAsync(DocumentoEmpresa documento)
    {
        context.DocumentosEmpresa.Update(documento);
        await context.SaveChangesAsync();
    }

    public async Task DesativarAsync(DocumentoEmpresa documento)
    {
        documento.Ativo = false;
        context.DocumentosEmpresa.Update(documento);
        await context.SaveChangesAsync();
    }
}