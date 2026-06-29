using AuthService.Domain;

namespace AuthService;

/// <summary>
/// DTO que el frontend espera (shared/types.ts -> AppUser).
/// id se serializa como string y role como 'admin' | 'user'.
/// </summary>
public record AppUserDto(string id, string name, string email, string? avatar_url, string role)
{
    public static AppUserDto From(Person p) => new(
        p.id.ToString(),
        p.name,
        p.email,
        p.avatar_url,
        MapRole(p.role));

    // El frontend solo distingue admin/user. 'A' => admin, el resto => user.
    public static string MapRole(char role) => role == 'A' ? "admin" : "user";

    // admin => 'A', user => 'U'.
    public static char ToDomainRole(string role) => role == "admin" ? 'A' : 'U';
}

public record LoginRequest(string email, string password);
public record RegisterRequest(string name, string email, string password, string? role);
public record UpdateProfileRequest(string name, string email, string? password, string? avatar_url);
public record SetRoleRequest(string actor_user_id, string role);
