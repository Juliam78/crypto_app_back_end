using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class MovementRepository : IMovementRepository
    {
        private readonly AppDbContext _context;

        public MovementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movement>> GetMovementsAsync()
        {
            return await _context.Movements.AsNoTracking().ToListAsync();
        }

        public async Task<Movement> GetMovementByIdAsync(int id)
        {
            return (await _context.Movements.FindAsync(id))!;
        }

        public async Task CreateMovementAsync(Movement movement)
        {
            movement.created_at = DateTime.UtcNow;
            await _context.Movements.AddAsync(movement);
            await _context.SaveChangesAsync();
        }
    }
}
