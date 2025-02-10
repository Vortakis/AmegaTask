using FinPriceFeed.Domain.Enum;

namespace FinPriceFeed.ExternalProviders.Tiingo
{
    public static class TiingoEndpoints
    {
        public static readonly Dictionary<MarketType, string> Instruments = new()
        {
            { MarketType.Stock, "iex" },
            { MarketType.Forex, "tiingo/fx" },
            { MarketType.Crypto, "tiingo/crypto" }
        };

        public static readonly Dictionary<MarketType, string> Prices = new()
        {
            { MarketType.Stock, "iex" },
            { MarketType.Forex, "tiingo/fx/top" },
            { MarketType.Crypto, "tiingo/crypto/top" }
        };

        public static readonly Dictionary<MarketType, string> WebSocket = new()
        {
            { MarketType.Stock, "iex" },
            { MarketType.Forex, "fx" },
            { MarketType.Crypto, "crypto" }
        };
    }
}
