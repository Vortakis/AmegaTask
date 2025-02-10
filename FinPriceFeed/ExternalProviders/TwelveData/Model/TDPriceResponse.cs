using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.Model
{
    public class TDPriceResponse : TDResponse
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("currency_base")]
        public string CurrencyBase { get; set; } = string.Empty;

        [JsonPropertyName("currency_quote")]
        public string CurrencyQuote { get; set; } = string.Empty;

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public int Timestamp { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
