using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Models;

public class Plano
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(80)]
    public string Nome { get; set; } = default!;

    public string? Descricao { get; set; }

    /// <summary>
    /// Duração do plano em dias (ex: 30, 90, 365)
    /// </summary>
    [Range(1, 5000)]
    public int DuracaoDias { get; set; }

    /// <summary>
    /// Valor do plano (você pode tratar como valor total do período, ou mensal — mas seja consistente)
    /// </summary>
    [Range(0, 999999)]
    public decimal Valor { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset AtualizadoEm { get; set; } = DateTimeOffset.UtcNow;
}
