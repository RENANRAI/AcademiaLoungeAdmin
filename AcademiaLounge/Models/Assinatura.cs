using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Models;

public enum StatusAssinatura
{
    ATIVA = 1,
    VENCIDA = 2,
    CANCELADA = 3
}

public class Assinatura
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AlunoId { get; set; }

    [Required]
    public Guid PlanoId { get; set; }

    public DateOnly DataInicio { get; set; }
    public DateOnly DataVencimento { get; set; }

    public StatusAssinatura Status { get; set; } = StatusAssinatura.ATIVA;

    public string? Observacoes { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset AtualizadoEm { get; set; } = DateTimeOffset.UtcNow;

    // Navegação (opcional, mas ajuda)
    public Aluno? Aluno { get; set; }
    public Plano? Plano { get; set; }
}
