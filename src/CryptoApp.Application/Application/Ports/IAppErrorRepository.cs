using CryptoAppBackEnd.Domains.Entities.Errors;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface IAppErrorRepository
    {
        Task CreateAsync(AppError error);

        /// <summary>Todos los errores, ordenados por fecha descendente.</summary>
        Task<IEnumerable<AppError>> GetAllAsync();
    }
}
