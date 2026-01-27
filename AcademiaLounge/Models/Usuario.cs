using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Models;

public enum PerfilUsuario
{
    ADMIN = 1,
    RECEPCAO = 2,
    PROFESSOR = 3
}

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(120)]
    public string Nome { get; set; } = default!;

    [MaxLength(180)]
    public string? Email { get; set; }

    [MaxLength(30)]
    public string? Telefone { get; set; }

    [Required, MaxLength(80)]
    public string Login { get; set; } = default!;

    [Required]
    public string SenhaHash { get; set; } = default!;

    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.ADMIN;

    public bool Ativo { get; set; } = true;

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset AtualizadoEm { get; set; } = DateTimeOffset.UtcNow;
}
