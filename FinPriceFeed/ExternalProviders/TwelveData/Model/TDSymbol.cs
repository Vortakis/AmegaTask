using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.Model
{
    public class TDSymbol
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = string.Empty;

        [JsonPropertyName("symbols")]
        public string Symbols { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
