using CryptoAppBackEnd.Domains.Entities.CryptoCurrencies;

namespace CryptoApp.Web.Contracts
{
    public record CreateCryptoCurrencyRequest(
        string Symbol,
        string Name,
        decimal CurrentPrice,
        decimal PriceChange24h,
        decimal MarketCap);

    public record UpdateCryptoCurrencyRequest(
        string Symbol,
        string Name,
        string ImageUrl,
        decimal CurrentPrice,
        decimal PriceChange24h,
        decimal MarketCap);

    public record CryptoCurrencyResponse(
        string Id,
        string Symbol,
        string Name,
        string ImageUrl,
        decimal CurrentPrice,
        decimal PriceChange24h,
        decimal MarketCap,
        string LastPriceUpdate,
        string CreatedAt,
        string UpdatedAt)
    {
        public static CryptoCurrencyResponse From(CryptoCurrency c) => new(
            c.id.ToString(),
            c.symbol,
            c.name,
            c.image_url,
            c.current_price,
            c.price_change_24h,
            c.market_cap,
            c.last_price_update.ToString("o"),
            c.created_at.ToString("o"),
            c.updated_at.ToString("o"));
    }
}
