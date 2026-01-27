using AcademiaLounge.Data;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int diasProximos = 7)
    {
        diasProximos = Math.Clamp(diasProximos, 1, 60);

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var ate = hoje.AddDays(diasProximos);
        var competenciaMes = $"{DateTime.UtcNow:yyyy-MM}";

        // ======================
        // CARDS
        // ======================

        var alunosAtivos = await _db.Alunos.AsNoTracking()
            .CountAsync(a => a.Status == StatusAluno.ATIVO);

        var assinaturasAtivas = await _db.Assinaturas.AsNoTracking()
            .CountAsync(a => a.Status == StatusAssinatura.ATIVA);

        var vencidas = await _db.Assinaturas.AsNoTracking()
            .CountAsync(a => a.Status == StatusAssinatura.ATIVA &&
                             a.DataVencimento < hoje);

        var vencendoHoje = await _db.Assinaturas.AsNoTracking()
            .CountAsync(a => a.Status == StatusAssinatura.ATIVA &&
                             a.DataVencimento == hoje);

        var recebidoMes = await _db.Pagamentos.AsNoTracking()
            .Where(p => p.Status == StatusPagamento.PAGO &&
                        p.Competencia == competenciaMes)
            .SumAsync(p => (decimal?)p.Valor) ?? 0m;

        var pendenteMes = await _db.Pagamentos.AsNoTracking()
            .Where(p => p.Status == StatusPagamento.PENDENTE &&
                        p.Competencia == competenciaMes)
            .SumAsync(p => (decimal?)p.Valor) ?? 0m;

        // ======================
        // LISTAS
        // ======================

        var venceHoje = await _db.Assinaturas.AsNoTracking()
            .Include(a => a.Aluno)
            .Include(a => a.Plano)
            .Where(a => a.Status == StatusAssinatura.ATIVA &&
                        a.DataVencimento == hoje)
            .OrderBy(a => a.Aluno!.Nome)
            .Take(20)
            .Select(a => new
            {
                a.Id,
                a.AlunoId,
                NomeAluno = a.Aluno!.Nome,
                Plano = a.Plano!.Nome,
                a.DataVencimento
            })
            .ToListAsync();

        var proximosVencimentos = await _db.Assinaturas.AsNoTracking()
            .Include(a => a.Aluno)
            .Include(a => a.Plano)
            .Where(a => a.Status == StatusAssinatura.ATIVA &&
                        a.DataVencimento > hoje &&
                        a.DataVencimento <= ate)
            .OrderBy(a => a.DataVencimento)
            .ThenBy(a => a.Aluno!.Nome)
            .Take(50)
            .Select(a => new
            {
                a.Id,
                a.AlunoId,
                NomeAluno = a.Aluno!.Nome,
                Plano = a.Plano!.Nome,
                a.DataVencimento
            })
            .ToListAsync();

        var inadimplentes = await _db.Assinaturas.AsNoTracking()
            .Include(a => a.Aluno)
            .Include(a => a.Plano)
            .Where(a => a.Status == StatusAssinatura.ATIVA &&
                        a.DataVencimento < hoje &&
                        a.Aluno!.Status != StatusAluno.CANCELADO)
            .OrderBy(a => a.DataVencimento)
            .Take(50)
            .Select(a => new
            {
                a.Id,
                a.AlunoId,
                NomeAluno = a.Aluno!.Nome,
                Plano = a.Plano!.Nome,
                a.DataVencimento
            })
            .ToListAsync();

        // ======================
        // RETORNO
        // ======================

        return Ok(new
        {
            referencia = new
            {
                hoje,
                ate,
                competenciaMes
            },
            cards = new
            {
                alunosAtivos,
                assinaturasAtivas,
                vencidas,
                vencendoHoje,
                recebidoMes,
                pendenteMes
            },
            listas = new
            {
                venceHoje,
                proximosVencimentos,
                inadimplentes
            }
        });
    }
}
