using Autenticador.API.Domain.Entidades;
using Autenticador.API.Domain.Interfaces;
using System.Data;

namespace Autenticador.API.Infrastructure.Persistencia
{
    // Implementação do repositório usando Dapper sobre um IDbConnection que lê do DataSet
    public class UsuarioRepositorioDapper : IUsuarioRepositorio
    {
        private readonly BancoMemoria _banco;

        public UsuarioRepositorioDapper(BancoMemoria banco)
        {
            _banco = banco;
        }

        public Task<Usuario?> ObterPorEmailAsync(string email)
        {
            var tabela = _banco.DataSet.Tables["Usuarios"];
            var rows = tabela.Select($"Email = '{Escape(email)}'");
            var row = rows.FirstOrDefault();
            if (row == null) return Task.FromResult<Usuario?>(null);
            var usuario = Mapear(row);
            return Task.FromResult<Usuario?>(usuario);
        }

        public Task<int> InserirAsync(Usuario usuario)
        {
            var tabela = _banco.DataSet.Tables["Usuarios"];
            // valida unicidade
            var existente = tabela.Select($"Email = '{Escape(usuario.Email)}'");
            if (existente.Any()) throw new InvalidOperationException("Já existe um usuário com este e-mail");

            var row = tabela.NewRow();
            row["Email"] = usuario.Email;
            row["SenhaHash"] = usuario.SenhaHash;
            row["CriadoEm"] = usuario.CriadoEm;
            tabela.Rows.Add(row);

            // forçar aceitação para gerar o Id autoincremento
            tabela.AcceptChanges();

            // O DataColumn AutoIncrement gera o valor somente após AcceptChanges quando se usa NewRow+Add
            // Recuperar o último Id
            var id = (int)row["Id"];
            return Task.FromResult(id);
        }

        public Task<Usuario?> ObterPorIdAsync(int id)
        {
            var tabela = _banco.DataSet.Tables["Usuarios"];
            var rows = tabela.Select($"Id = {id}");
            var row = rows.FirstOrDefault();
            if (row == null) return Task.FromResult<Usuario?>(null);
            var usuario = Mapear(row);
            return Task.FromResult<Usuario?>(usuario);
        }

        private Usuario Mapear(DataRow row)
        {
            return new Usuario
            {
                Id = Convert.ToInt32(row["Id"]),
                Email = Convert.ToString(row["Email"]) ?? string.Empty,
                SenhaHash = Convert.ToString(row["SenhaHash"]) ?? string.Empty,
                CriadoEm = Convert.ToDateTime(row["CriadoEm"])
            };
        }

        private static string Escape(string s)
        {
            return s.Replace("'", "''");
        }
    }
}
