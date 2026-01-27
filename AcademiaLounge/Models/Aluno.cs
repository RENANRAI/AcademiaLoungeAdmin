using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Models;
public enum StatusAluno
{
    ATIVO = 1,
    PAUSADO = 2,
    CANCELADO = 3
}

public class Aluno
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(120)]
    public string Nome { get; set; } = default!;

    [MaxLength(30)]
    public string? Telefone { get; set; }

    [MaxLength(180)]
    public string? Email { get; set; }

    public DateOnly? DataNascimento { get; set; }

    [MaxLength(30)]
    public string? Documento { get; set; }

    public StatusAluno Status { get; set; } = StatusAluno.ATIVO;

    public string? Observacoes { get; set; }

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset AtualizadoEm { get; set; } = DateTimeOffset.UtcNow;
}
