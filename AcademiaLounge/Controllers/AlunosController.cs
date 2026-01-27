using AcademiaLounge.Data;
using AcademiaLounge.Dtos;
using AcademiaLounge.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;
[Authorize]
[ApiController]
[Route("api/alunos")]
public class AlunosController : ControllerBase
{
    private readonly AppDbContext _db;

    public AlunosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlunoResponseDto>>> Get(
        [FromQuery] string? q,
        [FromQuery] StatusAluno? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Alunos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qLower = q.Trim().ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(qLower));
        }

        if (status.HasValue)
            query = query.Where(a => a.Status == status.Value);

        var itens = await query
            .OrderBy(a => a.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AlunoResponseDto(
                a.Id,
                a.Nome,
                a.Telefone,
                a.Email,
                a.DataNascimento,
                a.Documento,
                a.Status,
                a.Observacoes,
                a.CriadoEm,
                a.AtualizadoEm))
            .ToListAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlunoResponseDto>> GetById(Guid id)
    {
        var a = await _db.Alunos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (a is null) return NotFound();

        return Ok(new AlunoResponseDto(
            a.Id,
            a.Nome,
            a.Telefone,
            a.Email,
            a.DataNascimento,
            a.Documento,
            a.Status,
            a.Observacoes,
            a.CriadoEm,
            a.AtualizadoEm));
    }

    [HttpPost]
    public async Task<ActionResult<AlunoResponseDto>> Create([FromBody] AlunoCreateDto dto)
    {
        var aluno = new Aluno
        {
            Nome = dto.Nome.Trim(),
            Telefone = dto.Telefone?.Trim(),
            Email = dto.Email?.Trim(),
            DataNascimento = dto.DataNascimento,
            Documento = dto.Documento?.Trim(),
            Observacoes = dto.Observacoes,
            Status = StatusAluno.ATIVO,
            CriadoEm = DateTimeOffset.UtcNow,
            AtualizadoEm = DateTimeOffset.UtcNow
        };

        _db.Alunos.Add(aluno);
        await _db.SaveChangesAsync();

        var resp = new AlunoResponseDto(
            aluno.Id,
            aluno.Nome,
            aluno.Telefone,
            aluno.Email,
            aluno.DataNascimento,
            aluno.Documento,
            aluno.Status,
            aluno.Observacoes,
            aluno.CriadoEm,
            aluno.AtualizadoEm);

        return CreatedAtAction(nameof(GetById), new { id = aluno.Id }, resp);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AlunoUpdateDto dto)
    {
        var aluno = await _db.Alunos.FirstOrDefaultAsync(x => x.Id == id);
        if (aluno is null) return NotFound();

        aluno.Nome = dto.Nome.Trim();
        aluno.Telefone = dto.Telefone?.Trim();
        aluno.Email = dto.Email?.Trim();
        aluno.DataNascimento = dto.DataNascimento;
        aluno.Documento = dto.Documento?.Trim();
        aluno.Observacoes = dto.Observacoes;
        aluno.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] StatusAluno status)
    {
        var aluno = await _db.Alunos.FirstOrDefaultAsync(x => x.Id == id);
        if (aluno is null) return NotFound();

        aluno.Status = status;
        aluno.AtualizadoEm = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
