// src/SentinelaDocumentos.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SentinelaDocumentos.Domain.Entities; // Namespace das suas entidades

namespace SentinelaDocumentos.Infrastructure.Data;

// Herda de IdentityDbContext para incluir tabelas do Identity (Users, Roles, etc.)
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    // Mapeia suas entidades para tabelas no banco
    public DbSet<TipoDocumento> TiposDocumento { get; set; }
    public DbSet<DocumentoEmpresa> DocumentosEmpresa { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // ESSENCIAL: Chama a configuração base do IdentityDbContext
        base.OnModelCreating(builder);

        // Configurações adicionais usando Fluent API (alternativa/complemento aos DataAnnotations)
        builder.Entity<DocumentoEmpresa>(entity =>
        {
            // Garante que a chave estrangeira para ApplicationUser não pode ser nula
            // e configura a relação (embora o EF Core geralmente infira isso)
            entity.HasOne(d => d.Usuario)
                  .WithMany(u => u.Documentos) // Se você adicionou a coleção em ApplicationUser
                  .HasForeignKey(d => d.ApplicationUserId)
                  .IsRequired();

            // Garante que a chave estrangeira para TipoDocumento não pode ser nula
            entity.HasOne(d => d.TipoDocumento)
                  .WithMany() // TipoDocumento não tem coleção inversa
                  .HasForeignKey(d => d.TipoDocumentoId)
                  .IsRequired();

            // Cria índices para otimizar consultas frequentes
            entity.HasIndex(d => d.DataValidade).HasDatabaseName("IX_DocumentoEmpresa_DataValidade");
            entity.HasIndex(d => d.ApplicationUserId).HasDatabaseName("IX_DocumentoEmpresa_ApplicationUserId");
            entity.HasIndex(d => d.TipoDocumentoId).HasDatabaseName("IX_DocumentoEmpresa_TipoDocumentoId");

            // Poderia definir um índice único se fizesse sentido
            // Ex: entity.HasIndex(d => new { d.ApplicationUserId, d.TipoDocumentoId, d.NumeroDocumento }).IsUnique();
            // CUIDADO: NumeroDocumento pode ser nulo ou não único dependendo do tipo. Avaliar bem antes.
        });

        // --- Seeding (Pré-população) da tabela TipoDocumento ---
        builder.Entity<TipoDocumento>().HasData(
            new TipoDocumento { Id = 1, Nome = "CND Federal (RFB/PGFN)", PrazoAlertaPadraoDias = 30, Descricao = "Certidão Negativa de Débitos relativos a Créditos Tributários Federais e à Dívida Ativa da União" },
            new TipoDocumento { Id = 2, Nome = "CRF - FGTS (Caixa)", PrazoAlertaPadraoDias = 15, Descricao = "Certificado de Regularidade do FGTS" },
            new TipoDocumento { Id = 3, Nome = "CND Trabalhista (TST)", PrazoAlertaPadraoDias = 30, Descricao = "Certidão Negativa de Débitos Trabalhistas" },
            new TipoDocumento { Id = 4, Nome = "CND Estadual (ICMS/SEFAZ)", PrazoAlertaPadraoDias = 30, Descricao = "Certidão Negativa de Débitos Estaduais (pode variar por estado)" },
            new TipoDocumento { Id = 5, Nome = "CND Municipal (ISS/Pref.)", PrazoAlertaPadraoDias = 30, Descricao = "Certidão Negativa de Débitos Municipais (pode variar por município)" },
            new TipoDocumento { Id = 6, Nome = "Balanço Patrimonial", PrazoAlertaPadraoDias = 60, Descricao = "Último balanço registrado na Junta Comercial (validade conceitual)" },
            new TipoDocumento { Id = 7, Nome = "Atestado Capacidade Técnica", PrazoAlertaPadraoDias = null, Descricao = "Emitido por cliente anterior, validade depende do edital" },
            new TipoDocumento { Id = 8, Nome = "Certificado SICAF", PrazoAlertaPadraoDias = 30, Descricao = "Certificado de Registro Cadastral no SICAF" }
            // Adicione outros tipos relevantes que você identificar
        );
    }
}