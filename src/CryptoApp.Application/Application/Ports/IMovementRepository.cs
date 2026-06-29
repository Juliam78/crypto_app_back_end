using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IMovementRepository
    {
        /// <summary>Todos los movimientos, ordenados por fecha descendente (admin).</summary>
        Task<IEnumerable<Movement>> GetAllAsync();

        /// <summary>Movimientos de un usuario, ordenados por fecha descendente.</summary>
        Task<IEnumerable<Movement>> GetByUserAsync(string userId);

        /// <summary>
        /// Movimientos previos de un usuario+moneda+divisa, ordenados por fecha ascendente,
        /// para calcular el costo promedio ponderado de la posición.
        /// </summary>
        Task<IEnumerable<Movement>> GetPositionHistoryAsync(string userId, string coinId, string currency);

        Task CreateMovementAsync(Movement movement);
    }
}
