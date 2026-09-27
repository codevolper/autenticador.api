using Autenticador.API.Application.DTOs;
using Autenticador.API.Domain.Interfaces;
using Autenticador.API.Infrastructure.Seguranca;

namespace Autenticador.API.Application.CasosUso
{
    public class CasoDeUsoAutenticarUsuario
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly ServicoHashDeSenha _hasher;
        private readonly GeradorJwt _geradorJwt;

        public CasoDeUsoAutenticarUsuario(IUsuarioRepositorio repositorio, ServicoHashDeSenha hasher, GeradorJwt geradorJwt)
        {
            _repositorio = repositorio;
            _hasher = hasher;
            _geradorJwt = geradorJwt;
        }

        public async Task<string> ExecutarAsync(UsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                throw new ArgumentException("E-mail e Senha são obrigatórios");

            var usuario = await _repositorio.ObterPorEmailAsync(dto.Email);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");

            var valido = _hasher.VerificarHash(dto.Senha, usuario.SenhaHash);
            if (!valido)
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");

            var token = _geradorJwt.GerarToken(usuario);
            return token;
        }
    }
}
