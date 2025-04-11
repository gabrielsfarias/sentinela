using System.ComponentModel.DataAnnotations;

namespace SentinelaDocumentos.Domain.Entities;

public class TipoDocumento
{
    public int Id { get; set; }
    [Required(ErrorMessage = "O tipo de documento é obrigatório.")]
    [StringLength(100, ErrorMessage = "O tipo não pode exceder 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    public int? PrazoAlertaPadraoDias { get; set; }
}