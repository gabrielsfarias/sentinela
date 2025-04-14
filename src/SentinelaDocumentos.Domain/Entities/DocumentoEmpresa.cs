using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SentinelaDocumentos.Application.Services.Utils;

namespace SentinelaDocumentos.Domain.Entities;

public class DocumentoEmpresa
{
    public long Id { get; set; }
    
    [Required]
    public string ApplicationUserId { get; set; } = null!;
    [ForeignKey("ApplicationUserId")]
    public virtual ApplicationUser Usuario { get; set; } = null!;

    [Required]
    public int TipoDocumentoId {  get; set;}
    [ForeignKey("TipoDocumentoId")]
    public virtual TipoDocumento TipoDocumento { get; set; } = null!;

    [StringLength(150)]
    public string? OrgaoEmissor { get; set; }

    [StringLength(100)]
    public string? NumeroDocumento { get; set; } // Ex: Número da certidão

    public DateTime? DataEmissao { get; set; }

    [Required(ErrorMessage = "A data de validade é obrigatória.")]
    public DateTime DataValidade { get; set; } // Data que o documento expira

    public string? Observacoes { get; set; }

    public string? UrlAnexo { get; set; } // Caminho/URL do arquivo, se implementado upload

    public bool Ativo { get; set; } = true; // Para exclusão lógica (default é ativo)

    public DateTime DataRegistroUtc { get; set; } = DateTime.UtcNow; // Quando o registro foi criado/atualizado no sistema

    public DateTime? DataUltimoAlertaEnviado { get; set; } // Controle para não enviar alertas repetidos

    public string Status => DocumentoUtils.CalcularStatusDocumento(DataValidade);

    public int DiasParaVencer => (DataValidade - DateTime.UtcNow).Days;
}