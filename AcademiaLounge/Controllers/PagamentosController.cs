using AcademiaLounge.Data;
using AcademiaLounge.Dtos;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[Authorize] // admin only (depois você pode ajustar Roles)
[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PagamentosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PagamentoResponseDto>>> Get(
        [FromQuery] StatusPagamento? status,
        [FromQuery] string? competencia,
        [FromQuery] Guid? assinaturaId,
        [FromQuery] Guid? alunoId)
    {
        IQueryable<Pagamento> query = _db.Pagamentos.AsNoTracking()
            .Include(p => p.Assinatura)
            .ThenInclude(a => a.Aluno);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(competencia))
            query = query.Where(p => p.Competencia == competencia.Trim());

        if (assinaturaId.HasValue)
            query = query.Where(p => p.AssinaturaId == assinaturaId.Value);

        if (alunoId.HasValue)
            query = query.Where(p => p.Assinatura != null && p.Assinatura.AlunoId == alunoId.Value);

        var itens = await query
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => new PagamentoResponseDto(
                p.Id,
                p.AssinaturaId,
                p.Competencia,
                p.Valor,
                p.Forma,
                p.Status,
                p.DataPagamento,
                p.Observacoes,
                p.CriadoEm,
                p.AtualizadoEm,
                p.Assinatura != null ? p.Assinatura.AlunoId : null,
                p.Assinatura != null && p.Assinatura.Aluno != null ? p.Assinatura.Aluno.Nome : null
            ))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PagamentoResponseDto>> GetById(Guid id)
    {
        var p = await _db.Pagamentos.AsNoTracking()
            .Include(x => x.Assinatura)
            .ThenInclude(a => a.Aluno)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p is null) return NotFound();

        return Ok(new PagamentoResponseDto(
            p.Id, p.AssinaturaId, p.Competencia, p.Valor, p.Forma, p.Status, p.DataPagamento,
            p.Observacoes, p.CriadoEm, p.AtualizadoEm,
            p.Assinatura?.AlunoId,
            p.Assinatura?.Aluno?.Nome
        ));
    }

    [HttpPost]
    public async Task<ActionResult<PagamentoResponseDto>> Create([FromBody] PagamentoCreateDto dto)
    {
        var comp = (dto.Competencia ?? "").Trim();
        if (comp.Length != 7 || comp[4] != '-')
            return BadRequest("Competência inválida. Use YYYY-MM (ex: 2026-01).");

        var assinatura = await _db.Assinaturas
            .Include(a => a.Aluno)
            .FirstOrDefaultAsync(a => a.Id == dto.AssinaturaId);

        if (assinatura is null) return BadRequest("Assinatura não encontrada.");

        // regra simples
        if (assinatura.Status == StatusAssinatura.CANCELADA)
            return BadRequest("Não é permitido lançar pagamento para assinatura CANCELADA.");

        // evita duplicado (unique index também garante, mas aqui devolve mensagem melhor)
        var existe = await _db.Pagamentos.AnyAsync(p => p.AssinaturaId == dto.AssinaturaId && p.Competencia == comp);
        if (existe) return Conflict("Já existe pagamento lançado para essa assinatura e competência.");

        var status = dto.Status ?? StatusPagamento.PENDENTE;

        // se Status = PAGO e não veio data, coloca agora
        var dataPg = dto.DataPagamento;
        if (status == StatusPagamento.PAGO && dataPg is null)
            dataPg = DateTimeOffset.UtcNow;

        // se Status != PAGO, não guarda data
        if (status != StatusPagamento.PAGO)
            dataPg = null;

        var pagamento = new Pagamento
        {
            AssinaturaId = dto.AssinaturaId,
            Competencia = comp,
            Valor = dto.Valor,
            Forma = dto.Forma,
            Status = status,
            DataPagamento = dataPg,
            Observacoes = dto.Observacoes,
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        _db.Pagamentos.Add(pagamento);
        await _db.SaveChangesAsync();

        var resp = new PagamentoResponseDto(
            pagamento.Id,
            pagamento.AssinaturaId,
            pagamento.Competencia,
            pagamento.Valor,
            pagamento.Forma,
            pagamento.Status,
            pagamento.DataPagamento,
            pagamento.Observacoes,
            pagamento.CriadoEm,
            pagamento.AtualizadoEm,
            assinatura.AlunoId,
            assinatura.Aluno?.Nome
        );

        return CreatedAtAction(nameof(GetById), new { id = pagamento.Id }, resp);
    }

    [HttpPatch("{id:guid}/marcar-pago")]
    public async Task<IActionResult> MarcarPago(Guid id, [FromQuery] FormaPagamento? forma = null)
    {
        var p = await _db.Pagamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        if (p.Status != StatusPagamento.PAGO)
        {
            p.Status = StatusPagamento.PAGO;
            p.DataPagamento = DateTimeOffset.UtcNow;
            if (forma.HasValue) p.Forma = forma.Value;
            p.AtualizadoEm = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPatch("{id:guid}/cancelar")]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var p = await _db.Pagamentos.FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        p.Status = StatusPagamento.CANCELADO;
        p.DataPagamento = null;
        p.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
