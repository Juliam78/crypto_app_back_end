using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Movements;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
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
            var rows = await _context.Movements.AsNoTracking().ToListAsync();
            return rows.Select(MovementMapper.ToDomain).ToList();
        }

        public async Task<Movement> GetMovementByIdAsync(int id)
        {
            var row = await _context.Movements.FindAsync(id);
            return row is null ? null! : MovementMapper.ToDomain(row);
        }

        public async Task CreateMovementAsync(Movement movement)
        {
            await _context.Movements.AddAsync(MovementMapper.ToDb(movement));
            await _context.SaveChangesAsync();
        }
    }
}
