
using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;

namespace CryptoAppBackEnd.Application.Ports
{
    public interface ICryptoCurrencyRepository
    {
        Task<IEnumerable<CryptoCurrency>> GetCryptoCurrenciesAsync();
        Task<CryptoCurrency> GetCryptoCurrencyByIdAsync(int id);
        Task CreateCryptoCurrency(CryptoCurrency cryptoCurrency);
        Task UpdateCryptoCurrency(int id, CryptoCurrency cryptoCurrency);
        Task DeleteCryptoCurrency(int id);
    }
}