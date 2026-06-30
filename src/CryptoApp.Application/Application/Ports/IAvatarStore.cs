namespace CryptoAppBackEnd.Application.Ports
{
    /// <summary>
    /// DTO de almacenamiento de un avatar (no es entidad de dominio; equivale a <c>MarketCoin</c>
    /// como modelo de transporte entre el puerto y la capa Web).
    /// </summary>
    public record AvatarFile(byte[] Data, string ContentType);

    /// <summary>
    /// Puerto de salida para almacenar la imagen de avatar como binario en BD (tabla separada),
    /// evitando cargar los bytes en cada consulta de personas.
    /// </summary>
    public interface IAvatarStore
    {
        /// <summary>Crea o reemplaza (upsert) el avatar de la persona indicada.</summary>
        Task SaveAsync(int personId, byte[] data, string contentType, CancellationToken ct = default);

        /// <summary>Obtiene el avatar de la persona, o null si no existe.</summary>
        Task<AvatarFile?> GetAsync(int personId, CancellationToken ct = default);
    }
}
