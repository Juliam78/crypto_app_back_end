using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Mapeo de rol de persona entre el dominio (char) y el contrato del frontend (string).
    /// 'A' &lt;-&gt; "admin"; cualquier otro &lt;-&gt; "user".
    /// </summary>
    public static class RoleMapping
    {
        public const char AdminChar = 'A';
        public const char UserChar = 'U';

        public static string ToContract(char role) => role == AdminChar ? "admin" : "user";

        public static char ToDomain(string? role) =>
            string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) ? AdminChar : UserChar;
    }

    public record CreatePersonRequest(string Name, string Email, string Role);

    public record UpdatePersonRequest(string Name, string Email, string Role, bool Status);

    public record PersonResponse(
        string Id,
        string Name,
        string Email,
        string Role,
        bool Status,
        string CreatedAt,
        string UpdatedAt)
    {
        public static PersonResponse From(Person person) => new(
            person.id.ToString(),
            person.name,
            person.email,
            RoleMapping.ToContract(person.role),
            person.status,
            person.created_at.ToString("o"),
            person.updated_at.ToString("o"));
    }
}
