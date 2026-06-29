using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Infraestructure.Persistence;
using CryptoAppBackEnd.Infraestructure.Persistence.Mappers;
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
            var rows = await _context.Persons.AsNoTracking().ToListAsync();
            return rows.Select(PersonMapper.ToDomain).ToList();
        }

        public async Task<Person> GetPersonByIdAsync(int id)
        {
            var row = await _context.Persons.FindAsync(id);
            return row is null ? null! : PersonMapper.ToDomain(row);
        }

        public async Task CreatePersonAsync(Person person)
        {
            await _context.Persons.AddAsync(PersonMapper.ToDb(person));
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
            existing.updated_at = person.updated_at;

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
