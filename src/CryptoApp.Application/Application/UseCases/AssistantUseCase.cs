using System.Globalization;
using System.Text;
using CryptoAppBackEnd.Application.Market;
using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Academy;
using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoAppBackEnd.Application.UseCases
{
    /// <summary>Respuesta de la mascota IA. Source: "llm" (respondió el modelo) o "fallback" (determinista).</summary>
    public record AssistantAnswer(string Text, string Source);

    /// <summary>
    /// Caso de uso de la mascota IA asistente ("Cripto"). Fundamenta sus respuestas en datos reales
    /// (movimientos del usuario, mercado y señales publicadas) y delega la redacción en un
    /// <see cref="IAssistantProvider"/> compatible con OpenAI. Si el proveedor falla, devuelve una
    /// respuesta determinista construida con el mismo contexto, de modo que el endpoint nunca falle.
    /// </summary>
    public class AssistantUseCase
    {
        private const int MaxTopCoins = 8;

        private readonly MovementUseCase _movementUseCase;
        private readonly MarketUseCase _marketUseCase;
        private readonly LessonUseCase _lessonUseCase;
        private readonly IAssistantProvider _assistant;

        public AssistantUseCase(
            MovementUseCase movementUseCase,
            MarketUseCase marketUseCase,
            LessonUseCase lessonUseCase,
            IAssistantProvider assistant)
        {
            _movementUseCase = movementUseCase;
            _marketUseCase = marketUseCase;
            _lessonUseCase = lessonUseCase;
            _assistant = assistant;
        }

        /// <summary>
        /// Responde una pregunta del usuario. Construye contexto fundamentado, arma los prompts,
        /// intenta el LLM y, si falla, cae a una respuesta determinista. Nunca lanza por fallo del LLM.
        /// </summary>
        public async Task<AssistantAnswer> Ask(string question, string? coinId, Person? user, CancellationToken ct = default)
        {
            var context = await BuildContext(question, coinId, user, ct);

            var systemPrompt = BuildSystemPrompt();
            var userPrompt = BuildUserPrompt(question, context);

            try
            {
                var text = await _assistant.CompleteAsync(systemPrompt, userPrompt, ct);
                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new InvalidOperationException("El proveedor de IA devolvió una respuesta vacía.");
                }
                return new AssistantAnswer(text.Trim(), "llm");
            }
            catch
            {
                // Fallback determinista: el endpoint nunca falla aunque Ollama no esté instalado.
                return new AssistantAnswer(BuildFallbackText(context), "fallback");
            }
        }

        // --- Construcción de contexto fundamentado -------------------------------------------------

        private async Task<AssistantContext> BuildContext(string question, string? coinId, Person? user, CancellationToken ct)
        {
            var holdings = await BuildHoldings(user);
            var coins = await BuildMarket(coinId);
            var signals = await BuildSignals();

            return new AssistantContext(
                UserName: user?.name,
                Question: question,
                Holdings: holdings,
                Coins: coins,
                Signals: signals);
        }

        // Resume las tenencias del usuario por moneda a partir de sus movimientos (compras - ventas).
        private async Task<IReadOnlyList<HoldingSummary>> BuildHoldings(Person? user)
        {
            if (user is null) return Array.Empty<HoldingSummary>();

            // role "user" => el caso de uso filtra por su propio id (no expone movimientos ajenos).
            var movements = await _movementUseCase.GetMovements(user.id.ToString(CultureInfo.InvariantCulture), "user");

            var grouped = movements
                .GroupBy(m => new { m.coin_id, m.coin_symbol, m.coin_name })
                .Select(g =>
                {
                    decimal qty = 0;
                    decimal realizedPnl = 0;
                    foreach (var m in g)
                    {
                        qty += m.type == 'B' ? m.quantity : -m.quantity;
                        realizedPnl += m.realized_pnl;
                    }
                    return new HoldingSummary(
                        g.Key.coin_id,
                        g.Key.coin_symbol,
                        g.Key.coin_name,
                        Math.Max(0m, qty),
                        realizedPnl);
                })
                .Where(h => h.Quantity > 0m || h.RealizedPnl != 0m)
                .OrderByDescending(h => h.Quantity)
                .ToList();

            return grouped;
        }

        // Datos de mercado: si hay coinId, solo esa moneda; si no, las primeras MaxTopCoins.
        private async Task<IReadOnlyList<MarketCoin>> BuildMarket(string? coinId)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(coinId))
                {
                    var result = await _marketUseCase.GetCoinAsync("usd", coinId);
                    return result.Available && result.Value is not null
                        ? new[] { result.Value }
                        : Array.Empty<MarketCoin>();
                }

                var top = await _marketUseCase.GetTopCoinsAsync("usd");
                return top.Available
                    ? top.Value.Take(MaxTopCoins).ToList()
                    : Array.Empty<MarketCoin>();
            }
            catch
            {
                // El mercado nunca debe tumbar al asistente.
                return Array.Empty<MarketCoin>();
            }
        }

        private async Task<IReadOnlyList<Lesson>> BuildSignals()
        {
            try
            {
                var published = await _lessonUseCase.GetPublished();
                // Prioriza señales (con recomendación) pero incluye lecciones publicadas como contexto.
                return published
                    .OrderByDescending(l => l.kind == Lesson.KindSignal)
                    .ThenByDescending(l => l.updated_at)
                    .Take(10)
                    .ToList();
            }
            catch
            {
                return Array.Empty<Lesson>();
            }
        }

        // --- Prompts -------------------------------------------------------------------------------

        private static string BuildSystemPrompt()
        {
            return string.Join("\n", new[]
            {
                "Eres \"Cripto\", una mascota asistente de criptomonedas amistosa, cercana y motivadora.",
                "Respondes SIEMPRE en español, de forma breve y clara.",
                "REGLAS ESTRICTAS:",
                "- Usa EXCLUSIVAMENTE los datos proporcionados en el bloque de CONTEXTO. NO inventes precios, cifras ni señales.",
                "- Si te falta un dato para responder, dilo con honestidad en vez de inventarlo.",
                "- Puedes sugerir comprar/vender/mantener apoyándote en las señales y datos del contexto, explicando el porqué.",
                "- Cierra SIEMPRE recordando este descargo: esto es un simulador educativo y NO constituye asesoría financiera real."
            });
        }

        private static string BuildUserPrompt(string question, AssistantContext ctx)
        {
            var sb = new StringBuilder();
            sb.AppendLine("PREGUNTA DEL USUARIO:");
            sb.AppendLine(question.Trim());
            sb.AppendLine();
            sb.AppendLine("CONTEXTO (datos reales disponibles):");
            sb.Append(BuildContextBlock(ctx));
            return sb.ToString();
        }

        // Bloque compacto de contexto, reutilizado por el prompt del LLM y por el fallback.
        private static string BuildContextBlock(AssistantContext ctx)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Precios actuales (USD):");
            if (ctx.Coins.Count == 0)
            {
                sb.AppendLine("  (mercado no disponible)");
            }
            else
            {
                foreach (var c in ctx.Coins)
                {
                    sb.AppendLine(
                        $"  - {c.Name} ({c.Symbol.ToUpperInvariant()}): {FormatUsd(c.CurrentPrice)} " +
                        $"({FormatPct(c.PriceChangePercentage24h)} 24h)");
                }
            }

            sb.AppendLine("Tenencias del usuario:");
            if (ctx.Holdings.Count == 0)
            {
                sb.AppendLine(ctx.UserName is null
                    ? "  (usuario no identificado: sin portafolio)"
                    : "  (sin posiciones registradas)");
            }
            else
            {
                foreach (var h in ctx.Holdings)
                {
                    var pnl = h.RealizedPnl != 0m ? $", PnL realizado {FormatUsd(h.RealizedPnl)}" : string.Empty;
                    sb.AppendLine(
                        $"  - {h.CoinName} ({h.CoinSymbol.ToUpperInvariant()}): " +
                        $"{h.Quantity.ToString("0.########", CultureInfo.InvariantCulture)} unidades{pnl}");
                }
            }

            sb.AppendLine("Señales y lecciones publicadas:");
            if (ctx.Signals.Count == 0)
            {
                sb.AppendLine("  (sin señales publicadas)");
            }
            else
            {
                foreach (var s in ctx.Signals)
                {
                    if (s.kind == Lesson.KindSignal)
                    {
                        sb.AppendLine(
                            $"  - [SEÑAL {RecommendationText(s.recommendation)}] {s.coin_symbol?.ToUpperInvariant()}: {s.title}");
                    }
                    else
                    {
                        sb.AppendLine($"  - [LECCIÓN] {s.title}");
                    }
                    // Incluimos el contenido (truncado) para que el asistente pueda explicar la lección/señal.
                    if (!string.IsNullOrWhiteSpace(s.body))
                    {
                        sb.AppendLine($"      Contenido: {Truncate(s.body, 800)}");
                    }
                }
            }

            return sb.ToString();
        }

        // --- Fallback determinista -----------------------------------------------------------------

        private static string BuildFallbackText(AssistantContext ctx)
        {
            var sb = new StringBuilder();
            var name = string.IsNullOrWhiteSpace(ctx.UserName) ? string.Empty : $" {ctx.UserName}";
            sb.AppendLine($"¡Hola{name}! Soy Cripto. El asistente IA no está disponible ahora mismo, " +
                          "así que te comparto un resumen con los datos reales que tengo:");
            sb.AppendLine();
            sb.Append(BuildContextBlock(ctx));
            sb.AppendLine();
            sb.AppendLine("Recuerda: esto es un simulador educativo y NO constituye asesoría financiera real.");
            return sb.ToString();
        }

        // --- Helpers de formato --------------------------------------------------------------------

        private static string Truncate(string text, int max)
        {
            var clean = text.Replace("\r", " ").Replace("\n", " ").Trim();
            return clean.Length <= max ? clean : clean.Substring(0, max) + "…";
        }

        private static string FormatUsd(decimal value) =>
            "$" + value.ToString("0.########", CultureInfo.InvariantCulture);

        private static string FormatPct(decimal value) =>
            (value >= 0 ? "+" : string.Empty) + value.ToString("0.##", CultureInfo.InvariantCulture) + "%";

        private static string RecommendationText(char? recommendation) => recommendation switch
        {
            'B' => "COMPRAR",
            'S' => "VENDER",
            'H' => "MANTENER",
            _ => "?"
        };

        // --- Tipos internos de contexto ------------------------------------------------------------

        private sealed record AssistantContext(
            string? UserName,
            string Question,
            IReadOnlyList<HoldingSummary> Holdings,
            IReadOnlyList<MarketCoin> Coins,
            IReadOnlyList<Lesson> Signals);

        private sealed record HoldingSummary(
            string CoinId,
            string CoinSymbol,
            string CoinName,
            decimal Quantity,
            decimal RealizedPnl);
    }
}
