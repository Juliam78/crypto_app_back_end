using System.Security.Cryptography;
using System.Text;

namespace AuthService.Security;

/// <summary>
/// SHA-256 en hex minúscula sobre la contraseña sin espacios.
/// Mismo algoritmo que usaba el frontend (lib/auth.ts) para mantener compatibilidad.
/// Nota: SHA-256 sin sal es académico; en producción usar bcrypt/Argon2.
/// </summary>
public static class PasswordHasher
{
    public static string Hash(string password)
    {
        var normalized = (password ?? string.Empty).Trim();
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexStringLower(bytes);
    }
}
