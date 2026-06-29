using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Persons
{
    public class Person
    {
        public int id { get; private set; }
        public string name { get; private set; } = string.Empty;
        public string email { get; private set; } = string.Empty;
        public string password_hash { get; private set; } = string.Empty;
        public char role { get; private set; }
        public bool status { get; private set; }
        public string? avatar_url { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;
        public DateTime updated_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        private Person() { }

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
            this.created_at = DateTime.UtcNow;
            this.updated_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata una Person desde almacenamiento. Único punto autorizado para
        /// reconstituir el estado completo (incluidos id y timestamps) sin validación de negocio.
        /// </summary>
        public static Person FromPersistence(
            int id,
            string name,
            string email,
            string password_hash,
            char role,
            bool status,
            DateTime created_at,
            DateTime updated_at,
            string? avatar_url = null)
        {
            return new Person
            {
                id = id,
                name = name,
                email = email,
                password_hash = password_hash,
                role = role,
                status = status,
                created_at = created_at,
                updated_at = updated_at,
                avatar_url = avatar_url
            };
        }

        public void UpdateProfile(string name, string email, char role, bool status)
        {
            Helpers.ValidateFields(
                (nameof(name), name),
                (nameof(email), email),
                (nameof(role), role)
            );

            this.name = name;
            this.email = email;
            this.role = role;
            this.status = status;
            Touch();
        }

        public void ChangeRole(char role)
        {
            Helpers.ValidateFields((nameof(role), role));
            this.role = role;
            Touch();
        }

        public void SetAvatarUrl(string url)
        {
            Helpers.ValidateFields((nameof(url), url));
            this.avatar_url = url;
            Touch();
        }

        public void SetPasswordHash(string passwordHash)
        {
            Helpers.ValidateFields((nameof(passwordHash), passwordHash));
            this.password_hash = passwordHash;
            Touch();
        }

        public void Activate()
        {
            this.status = true;
            Touch();
        }

        public void Deactivate()
        {
            this.status = false;
            Touch();
        }

        private void Touch()
        {
            this.updated_at = DateTime.UtcNow;
        }
    }
}
