using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Portfolios
{
    public class Portfolio
    {
        public int id { get; set; }
        public int person_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string base_currency { get; set; } = string.Empty;
        public DateTime created_at { get; set; } = DateTime.UtcNow;
        public DateTime updated_at { get; set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core for materialization.
        public Portfolio() { }

        public Portfolio(string name, string base_currency)
        {
            Helpers.ValidateFields(
                (nameof(name), name),
                (nameof(base_currency), base_currency)
            );

            this.name = name;
            this.base_currency = base_currency;
        }
    }
}
