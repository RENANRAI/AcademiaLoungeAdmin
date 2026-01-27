using AcademiaLounge.Data;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[Authorize]
[ApiController]
[Route("api/relatorios")]
public class RelatoriosController : ControllerBase
{
    private readonly AppDbContext _db;

    public RelatoriosController(AppDbContext db) => _db = db;

    /// <summary>
    /// Inadimplentes: alunos com assinatura ATIVA e vencimento < hoje (e aluno não cancelado).
    /// </summary>
    [HttpGet("inadimplentes")]
    public async Task<IActionResult> Inadimplentes()
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        var itens = await _db.Assinaturas.AsNoTracking()
            .Include(a => a.Aluno)
            .Include(a => a.Plano)
            .Where(a => a.Status == StatusAssinatura.ATIVA &&
                        a.DataVencimento < hoje &&
                        a.Aluno != null &&
                        a.Aluno.Status != StatusAluno.CANCELADO)
            .OrderBy(a => a.DataVencimento)
            .Select(a => new
            {
                AlunoId = a.AlunoId,
                NomeAluno = a.Aluno!.Nome,
                Plano = a.Plano != null ? a.Plano.Nome : null,
                Vencimento = a.DataVencimento
            })
            .ToListAsync();

        return Ok(itens);
    }

    /// <summary>
    /// Receita mensal com base em pagamentos PAGO agrupados por competência.
    /// </summary>
    [HttpGet("receita-mensal")]
    public async Task<IActionResult> ReceitaMensal([FromQuery] int ano)
    {
        var prefixo = $"{ano}-"; // ex: 2026-

        var itens = await _db.Pagamentos.AsNoTracking()
            .Where(p => p.Status == StatusPagamento.PAGO && p.Competencia.StartsWith(prefixo))
            .GroupBy(p => p.Competencia)
            .Select(g => new
            {
                Competencia = g.Key,
                Total = g.Sum(x => x.Valor),
                Qtde = g.Count()
            })
            .OrderBy(x => x.Competencia)
            .ToListAsync();

        return Ok(itens);
    }
}
