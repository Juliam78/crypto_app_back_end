using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class PersonUseCase
    {
        private readonly IPersonRepository _personPort;

        public PersonUseCase(IPersonRepository personPort)
        {
            _personPort = personPort;
        }

        public async Task CreatePerson(Person person)
        {
            var existPerson = await _personPort.GetPersonByIdAsync(person.id);
            if (existPerson != null && existPerson.email == person.email)
            {
                throw new InvalidOperationException("Ya existe una persona relacionada con el email que intenta crear.");
            }

            await _personPort.CreatePersonAsync(person);
        }

        public async Task DeletePerson(int id)
        {
            if (await _personPort.GetPersonByIdAsync(id) == null)
            {
                throw new InvalidOperationException("La persona que intenta eliminar no existe.");
            }

            await _personPort.DeletePersonAsync(id);
        }

        public async Task<Person> GetPersonById(int id)
        {
            return await _personPort.GetPersonByIdAsync(id);
        }

        public async Task<IEnumerable<Person>> GetPersons()
        {
            return await _personPort.GetPersonsAsync();
        }

        public async Task UpdatePerson(Person person)
        {
            await _personPort.UpdatePersonAsync(person.id, person);
        }
    }
}
