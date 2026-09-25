using System;
using System.Security.Cryptography;

namespace Autenticador.API.Infrastructure.Seguranca
{
    // Implementação simples de hash seguro com PBKDF2
    public class ServicoHashDeSenha
    {
        private const int Iteracoes = 10000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string GerarHash(string senha)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, Iteracoes, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(HashSize);

            return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerificarHash(string senha, string hashArmazenado)
        {
            try
            {
                var partes = hashArmazenado.Split('.');
                if (partes.Length != 3) return false;
                var iter = int.Parse(partes[0]);
                var salt = Convert.FromBase64String(partes[1]);
                var hash = Convert.FromBase64String(partes[2]);

                using var pbkdf2 = new Rfc2898DeriveBytes(senha, salt, iter, HashAlgorithmName.SHA256);
                var computed = pbkdf2.GetBytes(hash.Length);

                return CryptographicOperations.FixedTimeEquals(computed, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
