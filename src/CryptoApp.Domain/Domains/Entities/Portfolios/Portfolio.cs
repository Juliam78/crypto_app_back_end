using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Portfolios
{
    public class Portfolio
    {
        public int id { get; private set; }
        public int person_id { get; private set; }
        public string name { get; private set; } = string.Empty;
        public string base_currency { get; private set; } = string.Empty;
        public DateTime created_at { get; private set; } = DateTime.UtcNow;
        public DateTime updated_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        private Portfolio() { }

        public Portfolio(string name, string base_currency)
        {
            Helpers.ValidateFields(
                (nameof(name), name),
                (nameof(base_currency), base_currency)
            );

            this.name = name;
            this.base_currency = base_currency;
            this.created_at = DateTime.UtcNow;
            this.updated_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata un Portfolio desde almacenamiento (estado completo, sin validación).
        /// </summary>
        public static Portfolio FromPersistence(
            int id,
            int person_id,
            string name,
            string base_currency,
            DateTime created_at,
            DateTime updated_at)
        {
            return new Portfolio
            {
                id = id,
                person_id = person_id,
                name = name,
                base_currency = base_currency,
                created_at = created_at,
                updated_at = updated_at
            };
        }

        public void AssignToPerson(int personId)
        {
            Helpers.ValidateFields((nameof(personId), personId));
            this.person_id = personId;
            Touch();
        }

        public void UpdateDetails(int person_id, string name, string base_currency)
        {
            Helpers.ValidateFields(
                (nameof(person_id), person_id),
                (nameof(name), name),
                (nameof(base_currency), base_currency)
            );

            this.person_id = person_id;
            this.name = name;
            this.base_currency = base_currency;
            Touch();
        }

        private void Touch()
        {
            this.updated_at = DateTime.UtcNow;
        }
    }
}
