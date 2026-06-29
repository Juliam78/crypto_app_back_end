using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// Puerto de salida para emitir y validar tokens de sesión.
    /// El adaptador concreto (HMAC-SHA256 autocontenido) vive en Infrastructure.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>Emite un token de sesión para la persona indicada.</summary>
        string Create(Person person);

        /// <summary>Devuelve el id del usuario si el token es válido y no expiró; null en caso contrario.</summary>
        int? Validate(string? token);
    }
}
