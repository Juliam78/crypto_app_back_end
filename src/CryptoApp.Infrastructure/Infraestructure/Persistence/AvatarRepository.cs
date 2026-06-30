using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Persistence
{
    /// <summary>
    /// Adaptador de almacenamiento de avatares en BD (implementa <see cref="IAvatarStore"/>).
    /// Guarda la imagen como binario en la tabla <c>avatars</c>, una fila por persona (upsert).
    /// </summary>
    public class AvatarRepository : IAvatarStore
    {
        private readonly AppDbContext _context;

        public AvatarRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(int personId, byte[] data, string contentType, CancellationToken ct = default)
        {
            var existing = await _context.Avatars
                .FirstOrDefaultAsync(a => a.person_id == personId, ct);

            if (existing is null)
            {
                _context.Avatars.Add(new AvatarDbModel
                {
                    person_id = personId,
                    data = data,
                    content_type = contentType,
                    updated_at = DateTime.UtcNow,
                });
            }
            else
            {
                existing.data = data;
                existing.content_type = contentType;
                existing.updated_at = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task<AvatarFile?> GetAsync(int personId, CancellationToken ct = default)
        {
            var row = await _context.Avatars.AsNoTracking()
                .FirstOrDefaultAsync(a => a.person_id == personId, ct);

            return row is null ? null : new AvatarFile(row.data, row.content_type);
        }
    }
}
