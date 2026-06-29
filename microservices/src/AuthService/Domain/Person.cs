namespace AuthService.Domain;

/// <summary>
/// Persona/usuario del sistema. Mantiene el estilo snake_case del monolito original.
/// role usa los chars del dominio: 'U' = usuario, 'A' = admin, 'E' = empleado.
/// </summary>
public class Person
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password_hash { get; set; } = string.Empty;
    public char role { get; set; }
    public bool status { get; set; } = true;
    public string? avatar_url { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime updated_at { get; set; } = DateTime.UtcNow;
}
