using System.Text.Json.Serialization;

namespace FinPriceFeed.ExternalProviders.TwelveData.Model
{
    public class TDResponse
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
    }
}
