using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CryptoAppBackEnd.Infraestructure.Adapters
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetPersonsAsync()
        {
            return await _context.Persons.AsNoTracking().ToListAsync();
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            return (await _context.Persons.FindAsync(id))!;
        }

        public async Task CreatePersonAsync(Person person)
        {
            person.created_at = DateTime.UtcNow;
            person.updated_at = DateTime.UtcNow;
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePersonAsync(int id, Person person)
        {
            var existing = await _context.Persons.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            existing.name = person.name;
            existing.email = person.email;
            existing.password_hash = person.password_hash;
            existing.role = person.role;
            existing.status = person.status;
            existing.updated_at = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePersonAsync(int id)
        {
            var existing = await _context.Persons.FindAsync(id);
            if (existing is null)
            {
                return;
            }

            _context.Persons.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
