using CryptoAppBackEnd.Domains.Shared;

namespace CryptoAppBackEnd.Domains.Entities.Academy
{
    /// <summary>
    /// Entidad académica única que cubre lecciones y señales de trading.
    /// kind: 'L' = lección, 'S' = señal. Cuando kind = 'S' la entidad exige una moneda
    /// (coin_id/coin_symbol) y una recomendación (recommendation: 'B' = buy, 'S' = sell, 'H' = hold).
    /// </summary>
    public class Lesson
    {
        public const char KindLesson = 'L';
        public const char KindSignal = 'S';

        public int id { get; private set; }
        public char kind { get; private set; }
        public string title { get; private set; } = string.Empty;
        public string body { get; private set; } = string.Empty;
        public string? coin_id { get; private set; }
        public string? coin_symbol { get; private set; }
        public char? recommendation { get; private set; }
        public string author_id { get; private set; } = string.Empty;
        public string author_name { get; private set; } = string.Empty;
        public bool published { get; private set; }
        public DateTime created_at { get; private set; } = DateTime.UtcNow;
        public DateTime updated_at { get; private set; } = DateTime.UtcNow;

        // Parameterless constructor required by EF Core / design-time materialization.
        private Lesson() { }

        /// <summary>
        /// Crea una lección o señal nueva, validada. title/body son obligatorios; si kind = 'S'
        /// se exige coin_id, coin_symbol y recommendation.
        /// </summary>
        public Lesson(
            char kind,
            string title,
            string body,
            string author_id,
            string author_name,
            string? coin_id = null,
            string? coin_symbol = null,
            char? recommendation = null)
        {
            Helpers.ValidateFields(
                (nameof(kind), kind),
                (nameof(title), title),
                (nameof(body), body),
                (nameof(author_id), author_id)
            );

            if (kind != KindLesson && kind != KindSignal)
            {
                throw new ArgumentException("kind debe ser 'L' (lección) o 'S' (señal).", nameof(kind));
            }

            if (kind == KindSignal)
            {
                ValidateSignal(coin_id, coin_symbol, recommendation);
            }

            this.kind = kind;
            this.title = title;
            this.body = body;
            this.author_id = author_id;
            this.author_name = author_name ?? string.Empty;
            this.coin_id = kind == KindSignal ? coin_id : null;
            this.coin_symbol = kind == KindSignal ? coin_symbol : null;
            this.recommendation = kind == KindSignal ? recommendation : null;
            this.published = false;
            this.created_at = DateTime.UtcNow;
            this.updated_at = DateTime.UtcNow;
        }

        /// <summary>
        /// Rehidrata una Lesson desde almacenamiento (estado completo, sin validación de negocio).
        /// </summary>
        public static Lesson FromPersistence(
            int id,
            char kind,
            string title,
            string body,
            string author_id,
            string author_name,
            bool published,
            DateTime created_at,
            DateTime updated_at,
            string? coin_id = null,
            string? coin_symbol = null,
            char? recommendation = null)
        {
            return new Lesson
            {
                id = id,
                kind = kind,
                title = title,
                body = body,
                author_id = author_id,
                author_name = author_name,
                published = published,
                created_at = created_at,
                updated_at = updated_at,
                coin_id = coin_id,
                coin_symbol = coin_symbol,
                recommendation = recommendation
            };
        }

        /// <summary>
        /// Actualiza el contenido. No cambia kind ni el estado de publicación. Si la lección es una
        /// señal, exige nuevamente coin_id/coin_symbol/recommendation válidos.
        /// </summary>
        public void UpdateContent(
            string title,
            string body,
            string? coin_id = null,
            string? coin_symbol = null,
            char? recommendation = null)
        {
            Helpers.ValidateFields(
                (nameof(title), title),
                (nameof(body), body)
            );

            if (kind == KindSignal)
            {
                ValidateSignal(coin_id, coin_symbol, recommendation);
            }

            this.title = title;
            this.body = body;
            this.coin_id = kind == KindSignal ? coin_id : null;
            this.coin_symbol = kind == KindSignal ? coin_symbol : null;
            this.recommendation = kind == KindSignal ? recommendation : null;
            Touch();
        }

        public void Publish()
        {
            this.published = true;
            Touch();
        }

        public void Unpublish()
        {
            this.published = false;
            Touch();
        }

        private static void ValidateSignal(string? coin_id, string? coin_symbol, char? recommendation)
        {
            if (string.IsNullOrWhiteSpace(coin_id) || string.IsNullOrWhiteSpace(coin_symbol))
            {
                throw new ArgumentException("Una señal requiere coin_id y coin_symbol.", nameof(coin_id));
            }

            if (recommendation is not ('B' or 'S' or 'H'))
            {
                throw new ArgumentException("Una señal requiere recommendation 'B', 'S' o 'H'.", nameof(recommendation));
            }
        }

        private void Touch()
        {
            this.updated_at = DateTime.UtcNow;
        }
    }
}
