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

        public async Task<IEnumerable<Movement>> GetAllAsync()
        {
            var rows = await _context.Movements.AsNoTracking()
                .OrderByDescending(m => m.created_at)
                .ToListAsync();
            return rows.Select(MovementMapper.ToDomain).ToList();
        }

        public async Task<IEnumerable<Movement>> GetByUserAsync(string userId)
        {
            var rows = await _context.Movements.AsNoTracking()
                .Where(m => m.user_id == userId)
                .OrderByDescending(m => m.created_at)
                .ToListAsync();
            return rows.Select(MovementMapper.ToDomain).ToList();
        }

        public async Task<IEnumerable<Movement>> GetPositionHistoryAsync(string userId, string coinId, string currency)
        {
            var rows = await _context.Movements.AsNoTracking()
                .Where(m => m.user_id == userId && m.coin_id == coinId && m.currency == currency)
                .OrderBy(m => m.created_at)
                .ToListAsync();
            return rows.Select(MovementMapper.ToDomain).ToList();
        }

        public async Task CreateMovementAsync(Movement movement)
        {
            await _context.Movements.AddAsync(MovementMapper.ToDb(movement));
            await _context.SaveChangesAsync();
        }
    }
}
