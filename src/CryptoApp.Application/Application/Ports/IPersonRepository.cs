using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetPersonsAsync();
        Task<Person> GetPersonByIdAsync(int id);
        Task<Person?> GetPersonByEmailAsync(string email);
        Task CreatePersonAsync(Person person);
        Task UpdatePersonAsync(int id, Person person);
        Task DeletePersonAsync(int id);
    }
}
