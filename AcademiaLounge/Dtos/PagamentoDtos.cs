using System.ComponentModel.DataAnnotations;
using AcademiaLounge.Models;

namespace AcademiaLounge.Dtos;

public record PagamentoCreateDto(
    [Required] Guid AssinaturaId,
    [Required, MaxLength(7)] string Competencia,
    [Range(0, 999999)] decimal Valor,
    FormaPagamento Forma,
    StatusPagamento? Status,
    DateTimeOffset? DataPagamento,
    string? Observacoes
);

public record PagamentoResponseDto(
    Guid Id,
    Guid AssinaturaId,
    string Competencia,
    decimal Valor,
    FormaPagamento Forma,
    StatusPagamento Status,
    DateTimeOffset? DataPagamento,
    string? Observacoes,
    DateTimeOffset CriadoEm,
    DateTimeOffset AtualizadoEm,
    Guid? AlunoId,
    string? NomeAluno
);
