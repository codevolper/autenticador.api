namespace Autenticador.API.Application.DTOs
{
    public class UsuarioCadastroDto
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class UsuarioLoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class UsuarioRespostaDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
