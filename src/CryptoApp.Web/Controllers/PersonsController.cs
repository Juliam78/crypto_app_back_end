using CryptoApp.Web.Contracts;
using CryptoAppBackEnd.Application.UseCases;
using CryptoAppBackEnd.Domains.Entities.Persons;
using Microsoft.AspNetCore.Mvc;

namespace CryptoApp.Web.Controllers
{
    [ApiController]
    [Route("api/persons")]
    public class PersonsController : ControllerBase
    {
        private readonly PersonUseCase _useCase;

        public PersonsController(PersonUseCase useCase)
        {
            _useCase = useCase;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonResponse>>> GetAll()
        {
            var persons = await _useCase.GetPersons();
            return Ok(persons.Select(PersonResponse.From));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PersonResponse>> GetById(int id)
        {
            var person = await _useCase.GetPersonById(id);
            if (person is null)
            {
                return NotFound(new { message = $"No existe una persona con id {id}." });
            }

            return Ok(PersonResponse.From(person));
        }

        [HttpPost]
        public async Task<ActionResult<PersonResponse>> Create([FromBody] CreatePersonRequest request)
        {
            try
            {
                var person = new Person(request.Name, request.Email, RoleMapping.ToDomain(request.Role));
                await _useCase.CreatePerson(person);
                return CreatedAtAction(nameof(GetById), new { id = person.id }, PersonResponse.From(person));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PersonResponse>> Update(int id, [FromBody] UpdatePersonRequest request)
        {
            var existing = await _useCase.GetPersonById(id);
            if (existing is null)
            {
                return NotFound(new { message = $"No existe una persona con id {id}." });
            }

            try
            {
                existing.UpdateProfile(request.Name, request.Email, RoleMapping.ToDomain(request.Role), request.Status);
                await _useCase.UpdatePerson(existing);
                return Ok(PersonResponse.From(existing));
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _useCase.DeletePerson(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
