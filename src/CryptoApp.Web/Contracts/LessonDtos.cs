using CryptoAppBackEnd.Domains.Entities.Academy;

namespace CryptoApp.Web.Contracts
{
    /// <summary>
    /// Mapeo de los enums char del dominio académico al contrato del frontend (string) y viceversa.
    /// kind: 'L' &lt;-&gt; "lesson", 'S' &lt;-&gt; "signal".
    /// recommendation: 'B' &lt;-&gt; "buy", 'S' &lt;-&gt; "sell", 'H' &lt;-&gt; "hold", null &lt;-&gt; null.
    /// </summary>
    public static class LessonMapping
    {
        public static string KindToContract(char kind) => kind == Lesson.KindSignal ? "signal" : "lesson";

        public static char KindToDomain(string? kind) =>
            string.Equals(kind, "signal", StringComparison.OrdinalIgnoreCase) ? Lesson.KindSignal : Lesson.KindLesson;

        public static string? RecommendationToContract(char? recommendation) => recommendation switch
        {
            'B' => "buy",
            'S' => "sell",
            'H' => "hold",
            _ => null
        };

        public static char? RecommendationToDomain(string? recommendation)
        {
            if (string.Equals(recommendation, "buy", StringComparison.OrdinalIgnoreCase)) return 'B';
            if (string.Equals(recommendation, "sell", StringComparison.OrdinalIgnoreCase)) return 'S';
            if (string.Equals(recommendation, "hold", StringComparison.OrdinalIgnoreCase)) return 'H';
            return null;
        }
    }

    /// <summary>
    /// DTO de salida. id como string; kind "lesson"/"signal"; recommendation "buy"/"sell"/"hold"/null;
    /// fechas en ISO 8601.
    /// </summary>
    public record LessonResponse(
        string id,
        string kind,
        string title,
        string body,
        string? coinId,
        string? coinSymbol,
        string? recommendation,
        string authorId,
        string authorName,
        bool published,
        string createdAt,
        string updatedAt)
    {
        public static LessonResponse From(Lesson l) => new(
            l.id.ToString(),
            LessonMapping.KindToContract(l.kind),
            l.title,
            l.body,
            l.coin_id,
            l.coin_symbol,
            LessonMapping.RecommendationToContract(l.recommendation),
            l.author_id,
            l.author_name,
            l.published,
            l.created_at.ToString("o"),
            l.updated_at.ToString("o"));
    }

    public record CreateLessonRequest(
        string kind,
        string title,
        string body,
        string? coinId,
        string? coinSymbol,
        string? recommendation);

    public record UpdateLessonRequest(
        string title,
        string body,
        string? coinId,
        string? coinSymbol,
        string? recommendation);
}
