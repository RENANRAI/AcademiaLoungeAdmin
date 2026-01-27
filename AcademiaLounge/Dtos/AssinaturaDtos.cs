using System.ComponentModel.DataAnnotations;
using AcademiaLounge.Models;

namespace AcademiaLounge.Dtos;

public record AssinaturaCreateDto(
    [Required] Guid AlunoId,
    [Required] Guid PlanoId,
    DateOnly? DataInicio,
    DateOnly? DataVencimento,
    string? Observacoes
);

public record AssinaturaResponseDto(
    Guid Id,
    Guid AlunoId,
    Guid PlanoId,
    DateOnly DataInicio,
    DateOnly DataVencimento,
    StatusAssinatura Status,
    string? Observacoes,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm,
    string? NomePlano,
    decimal? ValorPlano
);
