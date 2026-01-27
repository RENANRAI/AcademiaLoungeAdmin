using AcademiaLounge.Data;
using AcademiaLounge.Dtos;
using AcademiaLounge.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLounge.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwt;

    public AuthController(AppDbContext db, JwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }
    

    [AllowAnonymous]

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var login = dto.Login.Trim();

        var user = await _db.Usuarios.FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());
        if (user is null) return Unauthorized("Login ou senha inválidos.");

        if (!user.Ativo) return Unauthorized("Usuário desativado.");

        var ok = PasswordHasher.Verify(dto.Senha, user.SenhaHash);
        if (!ok) return Unauthorized("Login ou senha inválidos.");

        var (token, expiresAt) = _jwt.Generate(user);

        return Ok(new LoginResponseDto(
            Token: token,
            ExpiresAt: expiresAt,
            UserId: user.Id,
            Nome: user.Nome,
            Login: user.Login,
            Perfil: user.Perfil
        ));
    }
}
