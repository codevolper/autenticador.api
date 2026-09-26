using Autenticador.API.Application.CasosUso;
using Autenticador.API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Autenticador.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly CasoDeUsoCadastrarUsuario _casoCadastrar;
        private readonly CasoDeUsoAutenticarUsuario _casoAutenticar;
        private readonly Autenticador.API.Domain.Interfaces.IUsuarioRepositorio _repositorio;

        public UsuariosController(CasoDeUsoCadastrarUsuario casoCadastrar, CasoDeUsoAutenticarUsuario casoAutenticar, Autenticador.API.Domain.Interfaces.IUsuarioRepositorio repositorio)
        {
            _casoCadastrar = casoCadastrar;
            _casoAutenticar = casoAutenticar;
            _repositorio = repositorio;
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioCadastroDto dto)
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
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
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
    }
}
