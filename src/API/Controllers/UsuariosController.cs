using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Autenticador.API.Application.CasosUso;
using Autenticador.API.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autenticador.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly CasoDeUsoCadastrarUsuario _casoCadastrar;
        private readonly CasoDeUsoAutenticarUsuario _casoAutenticar;

        public UsuariosController(CasoDeUsoCadastrarUsuario casoCadastrar, CasoDeUsoAutenticarUsuario casoAutenticar)
        {
            _casoCadastrar = casoCadastrar;
            _casoAutenticar = casoAutenticar;
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
        public IActionResult ObterPerfil()
        {
            var sub = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == ClaimTypes.Name || c.Type == ClaimTypes.Email || c.Type == "sub");
            if (sub == null) return Unauthorized();
            return Ok(new { usuario = sub.Value });
        }
    }
}
