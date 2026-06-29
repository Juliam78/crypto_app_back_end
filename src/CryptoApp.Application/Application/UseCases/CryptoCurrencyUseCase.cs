using CryptoAppBackEnd.Application.Ports;
using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;

namespace CryptoAppBackEnd.Application.UseCases
{
    public class CryptoCurrencyUseCase
    {
        private readonly ICryptoCurrencyRepository _cryptoCurrencyPort;

        public CryptoCurrencyUseCase(ICryptoCurrencyRepository cryptoCurrencyPort)
        {
            _cryptoCurrencyPort = cryptoCurrencyPort;
        }

        public async Task<IEnumerable<CryptoCurrency>> GetPortfolio()
        {
            return await _cryptoCurrencyPort.GetCryptoCurrenciesAsync();
        }

        public async Task<CryptoCurrency> GetCryptoCurrencyById(int id)
        {
            return await _cryptoCurrencyPort.GetCryptoCurrencyByIdAsync(id);
        }

        public async Task CreateCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            await _cryptoCurrencyPort.CreateCryptoCurrency(cryptoCurrency);
        }

        public async Task DeleteCryptoCurrency(int id)
        {
            await _cryptoCurrencyPort.DeleteCryptoCurrency(id);
        }

        public async Task UpdateCryptoCurrency(CryptoCurrency cryptoCurrency)
        {
            await _cryptoCurrencyPort.UpdateCryptoCurrency(cryptoCurrency.id, cryptoCurrency);
        }
    }
}
