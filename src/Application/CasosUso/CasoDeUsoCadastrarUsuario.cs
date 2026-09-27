using Autenticador.API.Application.DTOs;
using Autenticador.API.Domain.Entidades;
using Autenticador.API.Domain.Interfaces;
using Autenticador.API.Infrastructure.Seguranca;

namespace Autenticador.API.Application.CasosUso
{
    public class CasoDeUsoCadastrarUsuario
    {
        private readonly IUsuarioRepositorio _repositorio;
        private readonly ServicoHashDeSenha _hasher;

        public CasoDeUsoCadastrarUsuario(IUsuarioRepositorio repositorio, ServicoHashDeSenha hasher)
        {
            _repositorio = repositorio;
            _hasher = hasher;
        }

        public async Task<UsuarioRespostaDto> ExecutarAsync(UsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("E-mail é obrigatório");

            if (string.IsNullOrWhiteSpace(dto.Senha))
                throw new ArgumentException("Senha é obrigatória");

            var existente = await _repositorio.ObterPorEmailAsync(dto.Email);
            if (existente != null)
                throw new InvalidOperationException("Já existe um usuário com este e-mail");

            var hash = _hasher.GerarHash(dto.Senha);

            var usuario = new Usuario
            {
                Email = dto.Email,
                SenhaHash = hash,
                CriadoEm = DateTime.UtcNow
            };

            var id = await _repositorio.InserirAsync(usuario);
            usuario.Id = id;

            return new UsuarioRespostaDto { Id = usuario.Id, Email = usuario.Email };
        }
    }
}
