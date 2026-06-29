namespace CryptoApp.Web.Contracts
{
    public record LoginRequest(string Email, string Password);

    public record RegisterRequest(string Name, string Email, string Password, string? Role);

    /// <summary>Respuesta de login/registro: usuario + token de sesión (contrato del frontend).</summary>
    public record AuthResponse(PersonResponse User, string Token);

    /// <summary>
    /// Cambio de rol. El frontend envía el id del actor (clave snake_case `actor_user_id`)
    /// y el nuevo rol ("admin"/"user").
    /// </summary>
    public record SetRoleRequest(string actor_user_id, string Role);
}
