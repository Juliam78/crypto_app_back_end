using CryptoAppBackEnd.Domains.Entities.Persons;
using CryptoAppBackEnd.Infraestructure.Persistence.Models;

namespace CryptoAppBackEnd.Infraestructure.Persistence.Mappers
{
    public static class PersonMapper
    {
        public static PersonDbModel ToDb(Person domain) => new()
        {
            id = domain.id,
            name = domain.name,
            email = domain.email,
            password_hash = domain.password_hash,
            role = domain.role,
            status = domain.status,
            created_at = domain.created_at,
            updated_at = domain.updated_at,
            avatar_url = domain.avatar_url
        };

        public static Person ToDomain(PersonDbModel db) => Person.FromPersistence(
            db.id,
            db.name,
            db.email,
            db.password_hash,
            db.role,
            db.status,
            db.created_at,
            db.updated_at,
            db.avatar_url);
    }
}
