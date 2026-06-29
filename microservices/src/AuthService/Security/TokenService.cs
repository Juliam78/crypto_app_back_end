using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AuthService.Security;

/// <summary>
/// Token de sesión autocontenido y firmado con HMAC-SHA256 (formato compacto payload.firma,
/// base64url). Lo emite y valida únicamente el auth-service. Permite restaurar la sesión tras
/// refrescar la página sin volver a pedir credenciales.
/// Nota: en producción conviene un JWT estándar validado también por el gateway/servicios.
/// </summary>
public class TokenService
{
    private readonly byte[] _secret;

    public TokenService(IConfiguration config)
    {
        var secret = config["Auth:TokenSecret"];
        if (string.IsNullOrWhiteSpace(secret)) secret = "dev-secret-cryptoapp-cambiar";
        _secret = Encoding.UTF8.GetBytes(secret);
    }

    public string Create(int userId, string email, string role, TimeSpan? lifetime = null)
    {
        var exp = DateTimeOffset.UtcNow.Add(lifetime ?? TimeSpan.FromDays(7)).ToUnixTimeSeconds();
        var payload = JsonSerializer.SerializeToUtf8Bytes(new TokenPayload(userId, email, role, exp));
        var payloadPart = Base64UrlEncode(payload);
        var signature = Base64UrlEncode(Sign(payloadPart));
        return $"{payloadPart}.{signature}";
    }

    /// <summary>Devuelve el id del usuario si el token es válido y no expiró; null en caso contrario.</summary>
    public int? Validate(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var parts = token.Split('.');
        if (parts.Length != 2) return null;

        var expectedSig = Base64UrlEncode(Sign(parts[0]));
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSig), Encoding.UTF8.GetBytes(parts[1])))
            return null;

        try
        {
            var payload = JsonSerializer.Deserialize<TokenPayload>(Base64UrlDecode(parts[0]));
            if (payload is null) return null;
            if (DateTimeOffset.FromUnixTimeSeconds(payload.exp) < DateTimeOffset.UtcNow) return null;
            return payload.sub;
        }
        catch
        {
            return null;
        }
    }

    private byte[] Sign(string data) => HMACSHA256.HashData(_secret, Encoding.UTF8.GetBytes(data));

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] Base64UrlDecode(string value)
    {
        value = value.Replace('-', '+').Replace('_', '/');
        value += (value.Length % 4) switch { 2 => "==", 3 => "=", _ => "" };
        return Convert.FromBase64String(value);
    }

    private record TokenPayload(int sub, string email, string role, long exp);
}
