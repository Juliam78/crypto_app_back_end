using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Persons;

namespace CryptoAppBackEnd.Application.UseCases
{
    /// <summary>
    /// Resultado de una operación de autenticación: la persona autenticada + su token de sesión.
    /// </summary>
    public record AuthResult(Person Person, string Token);

    /// <summary>
    /// Caso de uso de autenticación: registro, login, restauración de sesión y cambio de rol.
    /// Portado del microservicio AuthService respetando puertos/adaptadores.
    /// </summary>
    public class AuthUseCase
    {
        /// <summary>Correo del administrador maestro: único autorizado a cambiar roles.</summary>
        public const string originalAdminEmail = "admin@crypto.edu";

        private readonly IPersonRepository _personPort;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthUseCase(
            IPersonRepository personPort,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _personPort = personPort;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra una persona nueva. Valida email único; lanza InvalidOperationException si ya existe.
        /// role: "admin" => 'A', cualquier otro (o null) => 'U'.
        /// </summary>
        public async Task<AuthResult> Register(string name, string email, string password, string? role = null)
        {
            var existing = await _personPort.GetPersonByEmailAsync(email);
            if (existing is not null)
            {
                throw new InvalidOperationException("Ya existe un usuario con ese correo.");
            }

            var roleChar = string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) ? 'A' : 'U';
            var person = new Person(name, email, roleChar);
            person.SetPasswordHash(_passwordHasher.Hash(password));
            person.Activate();

            await _personPort.CreatePersonAsync(person);

            // Recupera la persona persistida para obtener el id asignado por la BD.
            var created = await _personPort.GetPersonByEmailAsync(email) ?? person;
            return new AuthResult(created, _tokenService.Create(created));
        }

        /// <summary>
        /// Autentica por email + password. Lanza UnauthorizedAccessException si las credenciales no son válidas.
        /// </summary>
        public async Task<AuthResult> Login(string email, string password)
        {
            var person = await _personPort.GetPersonByEmailAsync(email);
            if (person is null || !_passwordHasher.Verify(password, person.password_hash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            return new AuthResult(person, _tokenService.Create(person));
        }

        /// <summary>
        /// Restaura la sesión a partir del token. Devuelve la persona o null si el token es inválido/ausente.
        /// </summary>
        public async Task<Person?> GetCurrentUser(string? token)
        {
            var userId = _tokenService.Validate(token);
            if (userId is null) return null;

            var person = await _personPort.GetPersonByIdAsync(userId.Value);
            return person; // GetPersonByIdAsync devuelve null! cuando no existe
        }

        /// <summary>
        /// Cambia el rol de un usuario. Solo el admin maestro (originalAdminEmail) puede hacerlo.
        /// Lanza UnauthorizedAccessException si el actor no es el admin original,
        /// e InvalidOperationException si el usuario objetivo no existe.
        /// </summary>
        public async Task<Person> SetRole(int targetUserId, string newRole, string actorEmail)
        {
            if (!string.Equals(actorEmail, originalAdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("No autorizado para cambiar roles.");
            }

            var person = await _personPort.GetPersonByIdAsync(targetUserId);
            if (person is null)
            {
                throw new InvalidOperationException("El usuario que intenta modificar no existe.");
            }

            var roleChar = string.Equals(newRole, "admin", StringComparison.OrdinalIgnoreCase) ? 'A' : 'U';
            person.ChangeRole(roleChar);
            await _personPort.UpdatePersonAsync(person.id, person);

            return person;
        }
    }
}
