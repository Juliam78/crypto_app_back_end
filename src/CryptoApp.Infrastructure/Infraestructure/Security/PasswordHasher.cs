using System.Security.Cryptography;
using System.Text;
using CryptoAppBackEnd.Application.Ports;

namespace CryptoAppBackEnd.Infraestructure.Security
{
    /// <summary>
    /// SHA-256 en hex minúscula sobre la contraseña recortada (sin espacios al inicio/fin).
    /// Mismo algoritmo que el microservicio AuthService para mantener compatibilidad de hashes.
    /// Nota: SHA-256 sin sal es académico; en producción usar bcrypt/Argon2.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            var normalized = (password ?? string.Empty).Trim();
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
            return Convert.ToHexStringLower(bytes);
        }

        public bool Verify(string password, string hash)
        {
            var computed = Hash(password);
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(computed),
                Encoding.UTF8.GetBytes(hash ?? string.Empty));
        }
    }
}
