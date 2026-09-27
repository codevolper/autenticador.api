using Autenticador.API.Domain.Entidades;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Autenticador.API.Infrastructure.Seguranca
{
    public class GeradorJwt
    {
        private readonly IConfiguration _config;

        public GeradorJwt(IConfiguration config)
        {
            _config = config;
        }

        public string GerarToken(Usuario usuario)
        {
            var chave = _config["Jwt:ChaveSecreta"] ?? throw new InvalidOperationException("Chave JWT não configurada");
            var emissor = _config["Jwt:Emissor"] ?? string.Empty;
            var publico = _config["Jwt:Publico"] ?? string.Empty;
            var expiracaoMin = int.TryParse(_config["Jwt:ExpiracaoMinutos"], out var m) ? m : 60;

            var chaveBytes = Encoding.UTF8.GetBytes(chave);
            var credenciais = new SigningCredentials(new SymmetricSecurityKey(chaveBytes), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("nome", usuario.Email)
            };

            var token = new JwtSecurityToken(
                issuer: emissor,
                audience: publico,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiracaoMin),
                signingCredentials: credenciais
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
