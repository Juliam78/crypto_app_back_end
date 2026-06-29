using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IMovementRepository
    {
        Task<IEnumerable<Movement>> GetMovementsAsync();
        Task<Movement> GetMovementByIdAsync(int id);
        Task CreateMovementAsync(Movement movement);
    }
}
