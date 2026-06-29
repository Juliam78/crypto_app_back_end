using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Movements;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class MovementUseCase
    {
        private readonly IMovementRepository _movementPort;

        public MovementUseCase(IMovementRepository movementPort)
        {
            _movementPort = movementPort;
        }

        public async Task CreateMovement(Movement movement)
        {
            await _movementPort.CreateMovementAsync(movement);
        }
         public async Task<Movement> GetMovementsById(int portfolioId)
        {
            return await _movementPort.GetMovementByIdAsync(portfolioId);
        }

        public async Task<IEnumerable<Movement>> GetMovementsAsync()
        {
            return await _movementPort.GetMovementsAsync();
        }
    }
}
