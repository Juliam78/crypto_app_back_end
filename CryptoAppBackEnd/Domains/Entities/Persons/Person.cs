using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Persons
{
    public class Person
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password_hash { get; set; } = string.Empty;
        public char role { get; set; }
        public bool status { get; set; }
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        public Person() { }

        public Person(string name, string email, char role)
        {
            Helpers.ValidateFields(
                (nameof(name), name),
                (nameof(email), email),
                (nameof(role), role)
            );

            this.name = name;
            this.email = email;
            this.role = role;
        }
    }
}
