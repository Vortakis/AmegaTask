
using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.Model
{
    public class TDSubscriptionResponse : TDResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("success")]
        public TDSymbol[] Success { get; set; }

        [JsonPropertyName("fails")]
        public TDSymbol[] Fails { get; set; }
    }
}
