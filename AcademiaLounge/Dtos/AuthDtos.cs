using System.ComponentModel.DataAnnotations;
using AcademiaLounge.Models;

namespace AcademiaLounge.Dtos;

public record LoginRequestDto(
    [Required] string Login,
    [Required] string Senha
);

public record LoginResponseDto(
    string Token,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string Nome,
    string Login,
    PerfilUsuario Perfil
);
