using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Errors;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class AppErrorRepository : IAppErrorRepository
    {
        private readonly AppDbContext _context;

        public AppErrorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AppError error)
        {
            await _context.AppErrors.AddAsync(AppErrorMapper.ToDb(error));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppError>> GetAllAsync()
        {
            var rows = await _context.AppErrors.AsNoTracking()
                .OrderByDescending(e => e.created_at)
                .ToListAsync();
            return rows.Select(AppErrorMapper.ToDomain).ToList();
        }
    }
}
