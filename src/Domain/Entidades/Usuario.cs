namespace Autenticador.API.Domain.Entidades
{
    // Entidade de domínio representando o usuário do sistema
    public class Usuario
    {
        public int Id { get; set; }

        public string Email { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public DateTime CriadoEm { get; set; }
    }
}
