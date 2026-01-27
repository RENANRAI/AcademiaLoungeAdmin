using AcademiaLounge.Data;
using AcademiaLounge.Dtos;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[ApiController]
[Route("api/assinaturas")]
public class AssinaturasController : ControllerBase
{
    private readonly AppDbContext _db;

    public AssinaturasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssinaturaResponseDto>>> Get(
    [FromQuery] StatusAssinatura? status,
    [FromQuery] Guid? alunoId,
    [FromQuery] DateOnly? vencendoAte)
    {
        IQueryable<Assinatura> query = _db.Assinaturas.AsNoTracking();

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        if (alunoId.HasValue)
            query = query.Where(a => a.AlunoId == alunoId.Value);

        if (vencendoAte.HasValue)
            query = query.Where(a => a.DataVencimento <= vencendoAte.Value);

        var itens = await query
            .Include(a => a.Plano)
            .OrderByDescending(a => a.DataVencimento)
            .Select(a => new AssinaturaResponseDto(
                a.Id,
                a.AlunoId,
                a.PlanoId,
                a.DataInicio,
                a.DataVencimento,
                a.Status,
                a.Observacoes,
                a.CriadoEm,
                a.AtualizadoEm,
                a.Plano != null ? a.Plano.Nome : null,
                a.Plano != null ? a.Plano.Valor : null
            ))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AssinaturaResponseDto>> GetById(Guid id)
    {
        var a = await _db.Assinaturas
            .AsNoTracking()
            .Include(x => x.Plano)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (a is null) return NotFound();

        return Ok(new AssinaturaResponseDto(
            a.Id, a.AlunoId, a.PlanoId, a.DataInicio, a.DataVencimento, a.Status,
            a.Observacoes, a.CriadoEm, a.AtualizadoEm,
            a.Plano?.Nome, a.Plano?.Valor
        ));
    }

    [HttpPost]
    public async Task<ActionResult<AssinaturaResponseDto>> Create([FromBody] AssinaturaCreateDto dto)
    {
        // Aluno existe?
        var aluno = await _db.Alunos.FirstOrDefaultAsync(x => x.Id == dto.AlunoId);
        if (aluno is null) return BadRequest("Aluno não encontrado.");

        if (aluno.Status == StatusAluno.CANCELADO)
            return BadRequest("Não é permitido criar assinatura para aluno CANCELADO.");

        // Plano existe e está ativo?
        var plano = await _db.Planos.FirstOrDefaultAsync(x => x.Id == dto.PlanoId);
        if (plano is null) return BadRequest("Plano não encontrado.");

        if (!plano.Ativo)
            return BadRequest("Não é permitido criar assinatura para plano INATIVO.");

        var dataInicio = dto.DataInicio ?? DateOnly.FromDateTime(DateTime.UtcNow);

        // Se não informar vencimento, calcula pelo plano
        var dataVencimento = dto.DataVencimento ?? dataInicio.AddDays(plano.DuracaoDias);

        if (dataVencimento < dataInicio)
            return BadRequest("Data de vencimento não pode ser menor que a data de início.");

        var assinatura = new Assinatura
        {
            AlunoId = dto.AlunoId,
            PlanoId = dto.PlanoId,
            DataInicio = dataInicio,
            DataVencimento = dataVencimento,
            Status = StatusAssinatura.ATIVA,
            Observacoes = dto.Observacoes,
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        _db.Assinaturas.Add(assinatura);
        await _db.SaveChangesAsync();

        var resp = new AssinaturaResponseDto(
            assinatura.Id,
            assinatura.AlunoId,
            assinatura.PlanoId,
            assinatura.DataInicio,
            assinatura.DataVencimento,
            assinatura.Status,
            assinatura.Observacoes,
            assinatura.CriadoEm,
            assinatura.AtualizadoEm,
            plano.Nome,
            plano.Valor
        );

        return CreatedAtAction(nameof(GetById), new { id = assinatura.Id }, resp);
    }

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var assinatura = await _db.Assinaturas.FirstOrDefaultAsync(x => x.Id == id);
        if (assinatura is null) return NotFound();

        if (assinatura.Status != StatusAssinatura.CANCELADA)
        {
            assinatura.Status = StatusAssinatura.CANCELADA;
            assinatura.AtualizadoEm = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPatch("{id:guid}/renovar")]
    public async Task<IActionResult> Renovar(Guid id, [FromQuery] DateOnly? novaDataInicio)
    {
        var assinatura = await _db.Assinaturas.FirstOrDefaultAsync(x => x.Id == id);
        if (assinatura is null) return NotFound();

        var plano = await _db.Planos.FirstOrDefaultAsync(x => x.Id == assinatura.PlanoId);
        if (plano is null) return BadRequest("Plano da assinatura não encontrado.");

        if (!plano.Ativo)
            return BadRequest("Não é permitido renovar assinatura com plano INATIVO.");

        // Nova assinatura (histórico)
        var inicio = novaDataInicio ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var venc = inicio.AddDays(plano.DuracaoDias);

        var nova = new Assinatura
        {
            AlunoId = assinatura.AlunoId,
            PlanoId = assinatura.PlanoId,
            DataInicio = inicio,
            DataVencimento = venc,
            Status = StatusAssinatura.ATIVA,
            Observacoes = "Renovação",
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        _db.Assinaturas.Add(nova);

        // Opcional: marcar a anterior como VENCIDA se já passou do vencimento
        assinatura.AtualizadoEm = DateTimeOffset.UtcNow;
        if (assinatura.Status == StatusAssinatura.ATIVA)
            assinatura.Status = StatusAssinatura.VENCIDA;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
