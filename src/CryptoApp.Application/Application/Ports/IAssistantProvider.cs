namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// Puerto de salida hacia un proveedor de lenguaje (LLM) compatible con la API de OpenAI.
    /// El adaptador concreto (HttpClient contra <c>{BaseUrl}/chat/completions</c>) vive en Infrastructure.
    /// </summary>
    public interface IAssistantProvider
    {
        /// <summary>
        /// Genera una respuesta de texto a partir de un system prompt y un user prompt.
        /// Lanza excepción si el proveedor falla (HTTP no exitoso, respuesta vacía o sin conexión),
        /// para que el caso de uso pueda recurrir a su respuesta de fallback determinista.
        /// </summary>
        Task<string> CompleteAsync(string systemPrompt, string userPrompt, CancellationToken ct = default);
    }
}
