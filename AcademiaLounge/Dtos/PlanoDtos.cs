using System.ComponentModel.DataAnnotations;

namespace AcademiaLounge.Dtos;

public record PlanoCreateDto(
    [Required, MaxLength(80)] string Nome,
    string? Descricao,
    [Range(1, 5000)] int DuracaoDias,
    [Range(0, 999999)] decimal Valor
);

public record PlanoUpdateDto(
    [Required, MaxLength(80)] string Nome,
    string? Descricao,
    [Range(1, 5000)] int DuracaoDias,
    [Range(0, 999999)] decimal Valor,
    bool Ativo
);

public record PlanoResponseDto(
    Guid Id,
    string Nome,
    string? Descricao,
    int DuracaoDias,
    decimal Valor,
    bool Ativo,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm
);
