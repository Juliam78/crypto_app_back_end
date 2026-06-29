namespace CryptoApp.Web.Contracts
{
    /// <summary>Petición a la mascota IA: la pregunta y, opcionalmente, la moneda en foco.</summary>
    public record AskRequest(string question, string? coinId);

    /// <summary>Respuesta de la mascota IA. source: "llm" (modelo) o "fallback" (determinista).</summary>
    public record AssistantResponse(string answer, string source);
}
