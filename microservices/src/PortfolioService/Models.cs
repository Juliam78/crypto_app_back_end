namespace PortfolioService;

public class Portfolio
{
    public int id { get; set; }
    public int person_id { get; set; }
    public string name { get; set; } = string.Empty;
    public string base_currency { get; set; } = string.Empty;
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime updated_at { get; set; } = DateTime.UtcNow;

    public List<PortfolioAsset> assets { get; set; } = new();
}

public class PortfolioAsset
{
    public int id { get; set; }
    public int portfolio_id { get; set; }
    public string coin_id { get; set; } = string.Empty;
    public decimal quantity { get; set; }
    public decimal average_buy_price { get; set; }
    public decimal total_invested { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public DateTime updated_at { get; set; } = DateTime.UtcNow;
}

public record PortfolioRequest(int person_id, string name, string base_currency);
public record AssetRequest(string coin_id, decimal quantity, decimal average_buy_price, decimal total_invested);
