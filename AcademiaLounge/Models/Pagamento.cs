using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Models;

public enum StatusPagamento
{
    PENDENTE = 1,
    PAGO = 2,
    CANCELADO = 3
}

public enum FormaPagamento
{
    PIX = 1,
    DINHEIRO = 2,
    CARTAO = 3,
    TRANSFERENCIA = 4
}

public class Pagamento
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AssinaturaId { get; set; }

    /// <summary>Competência no formato YYYY-MM (ex: 2026-01)</summary>
    [Required, MaxLength(7)]
    public string Competencia { get; set; } = default!;

    [Range(0, 999999)]
    public decimal Valor { get; set; }

    public FormaPagamento Forma { get; set; } = FormaPagamento.PIX;

    public StatusPagamento Status { get; set; } = StatusPagamento.PENDENTE;

    public DateTimeOffset? DataPagamento { get; set; }

    public string? Observacoes { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset AtualizadoEm { get; set; } = DateTimeOffset.UtcNow;

    // Navegação
    public Assinatura? Assinatura { get; set; }
}
