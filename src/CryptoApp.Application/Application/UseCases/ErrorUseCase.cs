using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.Errors;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class ErrorUseCase
    {
        private readonly IAppErrorRepository _errorPort;

        public ErrorUseCase(IAppErrorRepository errorPort)
        {
            _errorPort = errorPort;
        }

        /// <summary>Registra un nuevo error reportado por el cliente.</summary>
        public async Task<AppError> LogError(
            string route,
            string message,
            string? stack = null,
            string? userId = null,
            string? userEmail = null)
        {
            var error = new AppError(route, message, stack, userId, userEmail);
            await _errorPort.CreateAsync(error);
            return error;
        }

        /// <summary>Lista de errores ordenados por fecha descendente.</summary>
        public async Task<IEnumerable<AppError>> GetErrors()
        {
            return await _errorPort.GetAllAsync();
        }
    }
}
