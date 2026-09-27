using Autenticador.API.Application.CasosUso;
using Autenticador.API.Application.DTOs;
using Autenticador.API.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Autenticador.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly CasoDeUsoCadastrarUsuario _casoCadastrar;
        private readonly CasoDeUsoAutenticarUsuario _casoAutenticar;
        private readonly IUsuarioRepositorio _repositorio;
        private readonly IConfiguration _config;

        public UsuariosController(CasoDeUsoCadastrarUsuario casoCadastrar, CasoDeUsoAutenticarUsuario casoAutenticar, IUsuarioRepositorio repositorio, IConfiguration config)
        {
            _casoCadastrar = casoCadastrar;
            _casoAutenticar = casoAutenticar;
            _repositorio = repositorio;
            _config = config;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioDto dto)
        {
            try
            {
                var resultado = await _casoCadastrar.ExecutarAsync(dto);
                return CreatedAtAction(nameof(ObterPerfil), new { id = resultado.Id }, resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { mensagem = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioDto dto)
        {
            try
            {
                var token = await _casoAutenticar.ExecutarAsync(dto);
                return Ok(new { access_token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObterPerfil()
        {
            // Extrair o claim 'sub' (subject) que contém o id do usuário
            var claimSub = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
            if (claimSub == null) return Unauthorized(new { mensagem = "Token inválido ou ausência de claim 'sub'" });

            if (!int.TryParse(claimSub.Value, out var usuarioId))
                return Unauthorized(new { mensagem = "Claim 'sub' inválido" });

            var usuario = await _repositorio.ObterPorIdAsync(usuarioId);
            if (usuario == null) return NotFound(new { mensagem = "Usuário não encontrado" });

            return Ok(new { id = usuario.Id, email = usuario.Email, criadoEm = usuario.CriadoEm });
        }

        [HttpPost("validar-token")]
        public IActionResult ValidarToken([FromBody] TokenValidacaoDto dto)
        {
            // Aceita token por body ou header Authorization: Bearer <token>
            var token = dto?.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer "))
                    token = authHeader.Substring("Bearer ".Length).Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { valido = false, mensagem = "Token não informado" });

            var chave = _config["Jwt:ChaveSecreta"] ?? string.Empty;
            var emissor = _config["Jwt:Emissor"] ?? string.Empty;
            var publico = _config["Jwt:Publico"] ?? string.Empty;
            var chaveBytes = Encoding.UTF8.GetBytes(chave);

            var tokenHandler = new JwtSecurityTokenHandler();
            var parametros = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(chaveBytes),
                ValidateIssuer = true,
                ValidIssuer = emissor,
                ValidateAudience = true,
                ValidAudience = publico,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, parametros, out var validatedToken);
                return Ok(new { valido = true });
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                return Ok(new { valido = false, mensagem = "Token expirado" });
            }
            catch (Exception)
            {
                return Ok(new { valido = false, mensagem = "Token inválido" });
            }
        }
    }
}
