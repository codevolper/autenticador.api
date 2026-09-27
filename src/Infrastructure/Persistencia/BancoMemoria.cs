using System.Data;

namespace Autenticador.API.Infrastructure.Persistencia
{
    // Banco em memória construído com DataSet e DataTable para simular tabelas relacionais
    public class BancoMemoria
    {
        public DataSet DataSet { get; }

        public BancoMemoria()
        {
            DataSet = new DataSet("Autenticador");
            CriarTabelaUsuarios();
        }

        private void CriarTabelaUsuarios()
        {
            var tabela = new DataTable("Usuarios");

            var colId = new DataColumn("Id", typeof(int)) { AutoIncrement = true, AutoIncrementSeed = 1, AutoIncrementStep = 1 };
            var colEmail = new DataColumn("Email", typeof(string));
            var colSenha = new DataColumn("SenhaHash", typeof(string));
            var colCriado = new DataColumn("CriadoEm", typeof(DateTime));

            tabela.Columns.Add(colId);
            tabela.Columns.Add(colEmail);
            tabela.Columns.Add(colSenha);
            tabela.Columns.Add(colCriado);

            tabela.PrimaryKey = new[] { colId };
            tabela.Constraints.Add(new UniqueConstraint("UC_Usuarios_Email", colEmail));

            DataSet.Tables.Add(tabela);
        }
    }
}
