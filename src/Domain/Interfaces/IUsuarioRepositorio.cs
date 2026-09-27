using Autenticador.API.Domain.Entidades;

namespace Autenticador.API.Domain.Interfaces
{
    // Repositório abstrato para operações com usuários
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> ObterPorEmailAsync(string email);

        Task<int> InserirAsync(Usuario usuario);

        Task<Usuario?> ObterPorIdAsync(int id);
    }
}
