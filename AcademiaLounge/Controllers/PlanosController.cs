
using AcademiaLounge.Data;
using AcademiaLounge.Dtos;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[ApiController]
[Route("api/planos")]
public class PlanosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlanosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlanoResponseDto>>> Get(
        [FromQuery] bool? ativo,
        [FromQuery] string? q)
    {
        var query = _db.Planos.AsNoTracking();

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Nome.ToLower().Contains(q.ToLower()));

        var itens = await query
            .OrderBy(p => p.Nome)
            .Select(p => new PlanoResponseDto(
                p.Id, p.Nome, p.Descricao, p.DuracaoDias, p.Valor, p.Ativo, p.CriadoEm, p.AtualizadoEm))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlanoResponseDto>> GetById(Guid id)
    {
        var p = await _db.Planos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p is null) return NotFound();

        return Ok(new PlanoResponseDto(
            p.Id, p.Nome, p.Descricao, p.DuracaoDias, p.Valor, p.Ativo, p.CriadoEm, p.AtualizadoEm));
    }
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult<PlanoResponseDto>> Create([FromBody] PlanoCreateDto dto)
    {
        // regra simples: não criar plano com nome duplicado (case-insensitive)
        var nome = dto.Nome.Trim();
        var existe = await _db.Planos.AnyAsync(x => x.Nome.ToLower() == nome.ToLower());
        if (existe) return Conflict($"Já existe um plano com o nome '{nome}'.");

        var plano = new Plano
        {
            Nome = nome,
            Descricao = dto.Descricao,
            DuracaoDias = dto.DuracaoDias,
            Valor = dto.Valor,
            Ativo = true
        };

        _db.Planos.Add(plano);
        await _db.SaveChangesAsync();

        var resp = new PlanoResponseDto(
            plano.Id, plano.Nome, plano.Descricao, plano.DuracaoDias, plano.Valor,
            plano.Ativo, plano.CriadoEm, plano.AtualizadoEm);

        return CreatedAtAction(nameof(GetById), new { id = plano.Id }, resp);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PlanoUpdateDto dto)
    {
        var plano = await _db.Planos.FirstOrDefaultAsync(x => x.Id == id);
        if (plano is null) return NotFound();

        var nome = dto.Nome.Trim();

        var nomeDuplicado = await _db.Planos.AnyAsync(x =>
            x.Id != id && x.Nome.ToLower() == nome.ToLower());

        if (nomeDuplicado) return Conflict($"Já existe outro plano com o nome '{nome}'.");

        plano.Nome = nome;
        plano.Descricao = dto.Descricao;
        plano.DuracaoDias = dto.DuracaoDias;
        plano.Valor = dto.Valor;
        plano.Ativo = dto.Ativo;
        plano.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        var plano = await _db.Planos.FirstOrDefaultAsync(x => x.Id == id);
        if (plano is null) return NotFound();

        if (!plano.Ativo)
        {
            plano.Ativo = true;
            plano.AtualizadoEm = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }

    [HttpPatch("{id:guid}/desativar")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var plano = await _db.Planos.FirstOrDefaultAsync(x => x.Id == id);
        if (plano is null) return NotFound();

        if (plano.Ativo)
        {
            plano.Ativo = false;
            plano.AtualizadoEm = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
        }

        return NoContent();
    }
}
