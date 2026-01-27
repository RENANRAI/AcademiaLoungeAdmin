using System.ComponentModel.DataAnnotations;
using AcademiaLounge.Models;


namespace AcademiaLounge.Dtos;

public record AlunoCreateDto(
    [Required, MaxLength(120)] string Nome,
    [MaxLength(30)] string? Telefone,
    [MaxLength(180)] string? Email,
    DateOnly? DataNascimento,
    [MaxLength(30)] string? Documento,
    string? Observacoes
);

public record AlunoUpdateDto(
    [Required, MaxLength(120)] string Nome,
    [MaxLength(30)] string? Telefone,
    [MaxLength(180)] string? Email,
    DateOnly? DataNascimento,
    [MaxLength(30)] string? Documento,
    string? Observacoes
);

public record AlunoResponseDto(
    Guid Id,
    string Nome,
    string? Telefone,
    string? Email,
    DateOnly? DataNascimento,
    string? Documento,
    StatusAluno Status,
    string? Observacoes,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm
);
