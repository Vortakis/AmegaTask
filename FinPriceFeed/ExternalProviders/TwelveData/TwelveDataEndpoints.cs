using FinPriceFeed.Domain.Enum;

namespace FinPriceFeed.ExternalProviders.TwelveData
{
    public static class TwelveDataEndpoints
    {
        public static readonly string WebSocket = "quotes/price";

        public static readonly string Price = "price";

        public static readonly Dictionary<MarketType, string> Instruments = new()
        {
            { MarketType.Stock, "stocks" },
            { MarketType.Forex, "forex_pairs" },
            { MarketType.Crypto, "cryptocurrencies" }
        };
    }
}
